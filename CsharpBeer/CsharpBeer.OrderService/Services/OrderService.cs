using Api;
using Grpc.Core;
using Common.Protobuf;
using CsharpBeer.OrderService.Domain.Common.Extensions.Orders;
using CsharpBeer.OrderService.Domain.Common.Interfaces;
using CsharpBeer.OrderService.Domain.Orders;
using CsharpBeer.OrderService.Services.Common.GrpcExtensions;
using Google.Protobuf.WellKnownTypes;
using CsharpBeer.OrderService.Services.Common.Errors;

namespace CsharpBeer.OrderService.Services;

public class OrderService(
    ILogger<OrderService> logger,
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository,
    IUnitOfWork unitOfWork,
    IAuthService authClient,
    ICatalogService catalogClient) : OrderServiceGrpc.OrderServiceGrpcBase
{
    private readonly ILogger<OrderService> _logger = logger;

    public override async Task<OrderDto> GetOrder(GetOrderRequest request, ServerCallContext context)
    {
        var (userId, _) = await PerformAuth(context);
        var order = await orderRepository.GetOrderByIdAsNoTrack(request.OrderId) ?? throw RpcErrors.OrderNotFound;

        if (!await IsOwnerOrAdmin(userId, order)) throw RpcErrors.PermissionDenied;

        var orderItems = await orderItemRepository.GetOrderItemsByOrderIdAsNoTrack(request.OrderId);
        
        var orderDto = order.ToDto();
        var beer = await GetOrderItemsDto(orderItems);
        orderDto.Items.AddRange(beer);
        
        return orderDto;
    }

    public override async Task<ListOrdersResponse> ListOrders(ListOrdersRequest request, ServerCallContext context)
    {
        var (userId, _) = await PerformAuth(context);

        var orders = request.UserId switch
        {
            null => await orderRepository.ListOrdersByUserIdAsNoTrack(userId),
            var otherUser when await authClient.IsAdminAsync(userId) => await orderRepository
                .ListOrdersByUserIdAsNoTrack(otherUser.Value),
            _ => throw RpcErrors.PermissionDenied
        };

        List<OrderDto> orderDtos = [];
        foreach (var order in orders)
        {
            var orderDto = order.ToDto();
            var orderItems = await orderItemRepository.GetOrderItemsByOrderIdAsNoTrack(order.OrderId);
            var beer = await GetOrderItemsDto(orderItems);
            orderDto.Items.AddRange(beer);
            orderDtos.Add(orderDto);
        }

        var response = new ListOrdersResponse();
        response.Orders.AddRange(orderDtos);
        return response;
    }

    public override async Task<OrderDto> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var (userId, _) = await PerformAuth(context);
        
        var beers = await catalogClient.GetBeersByIdAsync(request.Order.Items);
        var order = request.Order.ToDomain(userId, beers);
        await orderRepository.CreateOrder(order);

        await unitOfWork.CommitChangesAsync();

        return order.ToDto();
    }

    public override async Task<OrderDto> UpdateOrder(UpdateOrderRequest request, ServerCallContext context)
    {
        var (userId, _) = await PerformAuth(context);

        var orderInApp = await orderRepository.GetOrderById(request.Order.OrderId, true) ?? throw RpcErrors.OrderNotFound;
        if (!await IsOwnerOrAdmin(userId, orderInApp)) throw RpcErrors.PermissionDenied;
        
        var orderUpdate = request.Order.ToDomain();
        orderInApp.CopyFieldsIfNotNull(orderUpdate);

        await unitOfWork.CommitChangesAsync();

        var orderDto = orderInApp.ToDto();

        return orderDto;
    }

    public override async Task<Empty> DeleteOrder(DeleteOrderRequest request, ServerCallContext context)
    {
        var (userId, _) = await PerformAuth(context);

        var orderInApp = await orderRepository.GetOrderById(request.OrderId) ?? throw RpcErrors.OrderNotFound;
        if (!await IsOwnerOrAdmin(userId, orderInApp)) throw RpcErrors.PermissionDenied;
        
        await orderRepository.DeleteOrderById(orderInApp.OrderId);
        await orderItemRepository.DeleteAllOrderItemsByOrderId(orderInApp.OrderId);
        await unitOfWork.CommitChangesAsync();
        
        return new Empty();
    }

    private async Task<(long UserId, string Email)> PerformAuth(ServerCallContext context)
    {
        var token = context.GetToken();
        return await authClient.GetUserInfoAsync(token);
    }

    private async ValueTask<bool> IsOwnerOrAdmin(long userId, Order order)
    {
        if (userId == order.UserId) return true;
        return await authClient.IsAdminAsync(userId);
    }

    private async Task<List<OrderItemDto>> GetOrderItemsDto(IEnumerable<OrderItem> orderItems) => [.. 
        (await Task.WhenAll(orderItems.Select(async oi => {
            var orderItemDto = oi.ToDto();

            var beerResponse = await catalogClient.GetBeerByIdAsync(oi.BeerId);

            orderItemDto.Beer = beerResponse;
            return orderItemDto;
        })))
    ];

    private List<OrderItem> AddBeersToOrderItems(IEnumerable<Beer> beers, IEnumerable<CreateOrderItemDto> orderItems) => 
        beers.Zip(orderItems).Select(t => t.Second.ToDomain(t.First)).ToList();
}