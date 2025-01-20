using Api;
using Common.Protobuf;
using CsharpBeer.CommonTests;
using CsharpBeer.OrderService.Domain.Common.Interfaces;
using CsharpBeer.OrderService.Domain.Orders;
using CsharpBeer.OrderService.Infrastructure.Database;
using CsharpBeer.OrderService.Services.Common.Errors;
using FluentAssertions;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CsharpBeer.IntegrationTests;

public class OrderServiceTests : IClassFixture<OrderMicroserviceFactory>, IAsyncLifetime
{
    private readonly OrderMicroserviceFactory _fixture;
    private readonly ServerCallContext _defaultServerCallContext = 
        ServerCallContextFactory.Create(requestHeaders: ServerCallContextFactory.DefaultHeaders);

    public OrderServiceTests(OrderMicroserviceFactory fixture)
    {
        _fixture = fixture;
        DefaultMockSetup();
    }
    
    public async Task InitializeAsync()
    {
        await _fixture.SeedDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        _fixture.MockAuthService.Reset();
        _fixture.MockCatalogService.Reset();
        return Task.CompletedTask;
    }

    private static class TestConstants
    {
        public const long OrderId = 1;
        public const long UserId = 101;
        public const string UserEmail = "user@email.com";
        public const long OtherUserId = 106;
        public const int SeedDataOrderCount = 6;
        public const int SeedDataOrderItemsCount = 8;
    }
    
    private void DefaultMockSetup()
    {
        _fixture.MockAuthService
            .Setup(x => x.GetUserInfoAsync(It.IsAny<string?>()))
            .ReturnsAsync((TestConstants.UserId, TestConstants.UserEmail));
    }

    [Fact]
    public async Task CreateOrder()
    {
        // Arrange
        var beerId = 1;
        var request = new CreateOrderRequest
        {
            Order = new CreateOrderDto { Items = { new CreateOrderItemDto { BeerId = beerId } }}
        };
        List<Beer> beers = [new() { BeerId = beerId, }];
        _fixture.MockCatalogService
            .Setup(x => x.GetBeersByIdAsync(It.IsAny<IEnumerable<CreateOrderItemDto>>()))
            .ReturnsAsync(beers);
        
        // Act
        var result = await _fixture.GrpcClient.CreateOrderAsync(request, _defaultServerCallContext.RequestHeaders);
        
        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(TestConstants.UserId);
        result.Status.Should().Be(StatusOrderDto.Created);
        result.Items.Should().AllSatisfy(item => item.BeerId.Should().Be(beerId));
        await _fixture.DbCallAsync(async dbContext =>
        {
            (await dbContext.Orders.CountAsync()).Should().Be(TestConstants.SeedDataOrderCount + 1);
            var orderInDb = await dbContext.Orders.FirstAsync(order => order.OrderId == result.OrderId);
            orderInDb.Status.Should().Be(OrderStatus.Created);
            orderInDb.UserId.Should().Be(TestConstants.UserId);
            
            (await dbContext.OrderItems.CountAsync()).Should().Be(TestConstants.SeedDataOrderItemsCount + 1);
        });
    }
    
    [Fact]
    public async Task GetOrder_OrderDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var request = new GetOrderRequest { OrderId = 100 };
        
        //Act
        var ex = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.GrpcClient.GetOrderAsync(request, _defaultServerCallContext.RequestHeaders));
        
        // Assert
        ex.Status.Should().Be(RpcErrors.OrderNotFound.Status);
    }
    
    [Fact]
    public async Task ListOrders_GetMyOrders_ReturnsCorrectOrders()
    {
        // Arrange
        var request = new ListOrdersRequest();
        
        //Act
        var result = await _fixture.GrpcClient.ListOrdersAsync(request, _defaultServerCallContext.RequestHeaders);
        
        // Assert
        result.Orders.Count.Should().Be(2);
    }
    
    [Fact]
    public async Task ListOrders_GetOthersOrders_ReturnsCorrectOrders()
    {
        // Arrange
        var request = new ListOrdersRequest { UserId = TestConstants.OtherUserId };
        
        //Act
        var ex = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.GrpcClient.ListOrdersAsync(request, _defaultServerCallContext.RequestHeaders));
        
        // Assert
        ex.Status.Should().Be(RpcErrors.PermissionDenied.Status);
    }
    
    [Fact]
    public async Task ListOrders_AdminGetOthersOrder_ReturnsCorrectOrders()
    {
        // Arrange
        var request = new ListOrdersRequest() { UserId = TestConstants.OtherUserId };
        _fixture.MockAuthService.Setup(s => s.IsAdminAsync(It.IsAny<long>())).ReturnsAsync(true);
        
        //Act
        var result = await _fixture.GrpcClient.ListOrdersAsync(request, _defaultServerCallContext.RequestHeaders);
        
        // Assert
        result.Orders.Count.Should().Be(1);
        result.Orders.First().UserId.Should().Be(TestConstants.OtherUserId);
    }
    
    [Fact]
    public async Task GetOrder_OrderExist_ReturnsOrderDto()
    {
        // Arrange
        var request = new GetOrderRequest { OrderId = 1 };

        // Act
        var result = await _fixture.GrpcClient.GetOrderAsync(request, _defaultServerCallContext.RequestHeaders);
        
        // Assert
        result.OrderId.Should().Be(1);
        result.Items.Count.Should().Be(2);
    }
    
    [Fact]
    public async Task UpdateOrder_OrderExist_ReturnsOrderDto()
    {
        // Arrange
        var request = new UpdateOrderRequest
        {
            Order = new()
            {
                OrderId = 4,
                Status = StatusOrderDto.Delivered
            }
        };
        _fixture.MockAuthService
            .Setup(x => x.GetUserInfoAsync(It.IsAny<string?>()))
            .ReturnsAsync((104, TestConstants.UserEmail));
        
        // Act
        var result = await _fixture.GrpcClient.UpdateOrderAsync(request, _defaultServerCallContext.RequestHeaders);
        
        // Assert
        result.Status.Should().Be(StatusOrderDto.Delivered);
        await _fixture.DbCallAsync(async dbContext =>
        {
            (await dbContext.Orders.FirstAsync(order => order.OrderId == result.OrderId))
                .Status.Should().Be(OrderStatus.Delivered);
        });
    }
    
    [Fact]
    public async Task UpdateOrder_OrderDoesNotExist_NotFound()
    {
        // Arrange
        var request = new UpdateOrderRequest { Order = new() { OrderId = 10 } };
        
        // Act
        var ex = await Assert.ThrowsAsync<RpcException>(async () =>
            await _fixture.GrpcClient.UpdateOrderAsync(request, _defaultServerCallContext.RequestHeaders));
        
        // Assert
        ex.Status.Should().Be(RpcErrors.OrderNotFound.Status);
    }
    
    [Fact]
    public async Task DeleteOrder_OrderExist_Deleted()
    {
        // Arrange
        var request = new DeleteOrderRequest { OrderId = 1 };
        
        // Act
        var result = await _fixture.GrpcClient.DeleteOrderAsync(request, _defaultServerCallContext.RequestHeaders);
        
        // Assert
        await _fixture.DbCallAsync(async dbContext =>
        {
            (await dbContext.Orders.FirstOrDefaultAsync(order => order.OrderId == 1))
                .Should().BeNull();
        });
    }
    
    [Fact]
    public async Task DeleteOrder_OrderDoesNotExist_NotFound()
    {
        // Arrange
        var request = new DeleteOrderRequest { OrderId = 10 };

        // Act
        var ex = await Assert.ThrowsAsync<RpcException>(async () =>
            await _fixture.GrpcClient.DeleteOrderAsync(request, _defaultServerCallContext.RequestHeaders));
        
        // Assert
        ex.Status.Should().Be(RpcErrors.OrderNotFound.Status);
        await _fixture.DbCallAsync(async dbContext =>
        {
            (await dbContext.Orders.CountAsync()).Should().Be(TestConstants.SeedDataOrderCount);
        });
    }
    
    [Fact]
    public async Task AnyCall_InvalidToken_ReturnsPermissionDenied()
    {
        // Arrange
        var getRequest = new GetOrderRequest { OrderId = TestConstants.OrderId };
        var updateRequest = new UpdateOrderRequest { Order = new () { OrderId = TestConstants.OrderId } };
        var deleteRequest = new DeleteOrderRequest { OrderId = TestConstants.OrderId };
        _fixture.MockAuthService.Setup(a => a.GetUserInfoAsync(It.IsAny<string?>()))
            .ReturnsAsync((TestConstants.OtherUserId, TestConstants.UserEmail));

        // Act
        List<RpcException> responses =
        [
            await Assert.ThrowsAsync<RpcException>(async () =>
                await _fixture.GrpcClient.GetOrderAsync(getRequest, _defaultServerCallContext.RequestHeaders)),
            await Assert.ThrowsAsync<RpcException>(async () =>
                await _fixture.GrpcClient.UpdateOrderAsync(updateRequest, _defaultServerCallContext.RequestHeaders)),
            await Assert.ThrowsAsync<RpcException>(async () =>
                await _fixture.GrpcClient.DeleteOrderAsync(deleteRequest, _defaultServerCallContext.RequestHeaders))
        ];
        
        // Assert
        responses.Should().AllSatisfy(ex => ex.Status.Should().Be(RpcErrors.PermissionDenied.Status));
    }
}