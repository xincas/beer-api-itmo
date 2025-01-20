using Api;
using Common.Protobuf;
using CsharpBeer.CommonTests;
using CsharpBeer.OrderService.Domain.Common.Interfaces;
using CsharpBeer.OrderService.Domain.Orders;
using CsharpBeer.OrderService.Services.Common.Errors;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace CsharpBeer.UnitTests.Application;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IOrderItemRepository> _mockOrderItemRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ICatalogService> _mockCatalogService;
    private readonly Mock<ILogger<OrderService.Services.OrderService>> _mockLogger;
    private readonly OrderService.Services.OrderService _orderService;

    private readonly ServerCallContext _defaultServerCallContext;

    private static class TestConstants
    {
        public const long OrderId = 1;
        public const long UserId = 10;
        public const string UserEmail = "user@email.com";
        public const long OtherUserId = 11;
    }

    public OrderServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockOrderItemRepository = new Mock<IOrderItemRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAuthService = new Mock<IAuthService>();
        _mockCatalogService = new Mock<ICatalogService>();
        _mockLogger = new Mock<ILogger<OrderService.Services.OrderService>>();
        _defaultServerCallContext = ServerCallContextFactory.Create(requestHeaders: ServerCallContextFactory.DefaultHeaders);

        _orderService = new OrderService.Services.OrderService(
            _mockLogger.Object,
            _mockOrderRepository.Object,
            _mockOrderItemRepository.Object,
            _mockUnitOfWork.Object,
            _mockAuthService.Object,
            _mockCatalogService.Object
        );
        
        DefaultMockSetup();
    }
    
    private void DefaultMockSetup()
    {
        _mockAuthService
            .Setup(x => x.GetUserInfoAsync(It.IsAny<string?>()))
            .ReturnsAsync((TestConstants.UserId, TestConstants.UserEmail));
    }

    [Fact]
    public async Task GetOrder_OrderExistsAndUserHasPermission_ReturnOrderDto()
    {
        // Arrange
        var mockOrder = new Order { UserId = TestConstants.UserId };
        var request = new GetOrderRequest { OrderId = TestConstants.OrderId };
        
        _mockAuthService
            .Setup(x => x.IsAdminAsync(It.IsAny<long>()))
            .ReturnsAsync(false);
        _mockOrderRepository
            .Setup(x => x.GetOrderByIdAsNoTrack(It.IsAny<long>()))
            .ReturnsAsync(mockOrder);
        _mockOrderItemRepository
            .Setup(x => x.GetOrderItemsByOrderIdAsNoTrack(It.IsAny<long>()))
            .ReturnsAsync([]);

        // Act
        var result = await _orderService.GetOrder(request, _defaultServerCallContext);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(TestConstants.UserId);
        _mockOrderRepository.Verify(x => x.GetOrderByIdAsNoTrack(request.OrderId), Times.Once);
        _mockOrderItemRepository.Verify(x => x.GetOrderItemsByOrderIdAsNoTrack(request.OrderId), Times.Once);
    }

    [Fact]
    public async Task GetOrder_UserHasNoPermission_ThrowPermissionDenied()
    {
        // Arrange
        var request = new GetOrderRequest { OrderId = TestConstants.OrderId };
        
        _mockAuthService
            .Setup(x => x.IsAdminAsync(It.IsAny<long>()))
            .ReturnsAsync(false);

        var mockOrder = new Order { UserId = TestConstants.OtherUserId };
        _mockOrderRepository
            .Setup(x => x.GetOrderByIdAsNoTrack(It.IsAny<long>()))
            .ReturnsAsync(mockOrder);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _orderService.GetOrder(request, _defaultServerCallContext));
        ex.Status.Should().Be(RpcErrors.PermissionDenied.Status);
    }
    
    [Theory]
    [InlineData(TestConstants.UserId)]
    [InlineData(TestConstants.OtherUserId)]
    public async Task GetOrder_UserIsAdmin_GetAnyOrder(long userId)
    {
        // Arrange
        var mockOrder = new Order { UserId = userId };
        var request = new GetOrderRequest { OrderId = TestConstants.OrderId };
        
        _mockAuthService
            .Setup(x => x.IsAdminAsync(It.IsAny<long>()))
            .ReturnsAsync(true);
        _mockOrderRepository
            .Setup(x => x.GetOrderByIdAsNoTrack(It.IsAny<long>()))
            .ReturnsAsync(mockOrder);
        _mockOrderItemRepository
            .Setup(x => x.GetOrderItemsByOrderIdAsNoTrack(It.IsAny<long>()))
            .ReturnsAsync([]);

        // Act
        var result = await _orderService.GetOrder(request, _defaultServerCallContext);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        _mockOrderRepository.Verify(x => x.GetOrderByIdAsNoTrack(request.OrderId), Times.Once);
        _mockOrderItemRepository.Verify(x => x.GetOrderItemsByOrderIdAsNoTrack(request.OrderId), Times.Once);
    }

        [Fact]
    public async Task DeleteOrder_OrderExistsAndUserHasPermission_DeleteOrder()
    {
        // Arrange
        var mockOrder = new Order { UserId = TestConstants.UserId };
        var request = new DeleteOrderRequest() { OrderId = TestConstants.OrderId };
        
        _mockAuthService
            .Setup(x => x.IsAdminAsync(It.IsAny<long>()))
            .ReturnsAsync(false);
        _mockOrderRepository
            .Setup(x => x.GetOrderById(It.IsAny<long>(), It.IsAny<bool>()))
            .ReturnsAsync(mockOrder);

        // Act
        var result = await _orderService.DeleteOrder(request, _defaultServerCallContext);

        // Assert
        result.Should().BeEquivalentTo(new Empty());
        _mockOrderRepository.Verify(x => x.GetOrderById(request.OrderId, It.IsAny<bool>()), Times.Once);
        _mockOrderRepository.Verify(x => x.DeleteOrderById(It.IsAny<long>()), Times.Once);
        _mockOrderItemRepository.Verify(x => x.DeleteAllOrderItemsByOrderId(It.IsAny<long>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteOrder_UserHasNoPermission_ThrowPermissionDenied()
    {
        // Arrange
        var request = new DeleteOrderRequest() { OrderId = TestConstants.OrderId };
        
        _mockAuthService
            .Setup(x => x.IsAdminAsync(It.IsAny<long>()))
            .ReturnsAsync(false);

        var mockOrder = new Order { UserId = TestConstants.OtherUserId };
        _mockOrderRepository
            .Setup(x => x.GetOrderById(It.IsAny<long>(), It.IsAny<bool>()))
            .ReturnsAsync(mockOrder);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _orderService.DeleteOrder(request, _defaultServerCallContext));
        ex.Status.Should().Be(RpcErrors.PermissionDenied.Status);
    }
    
    [Theory]
    [InlineData(TestConstants.UserId)]
    [InlineData(TestConstants.OtherUserId)]
    public async Task DeleteOrder_UserIsAdmin_DeleteAnyOrder(long userId)
    {
        // Arrange
        var mockOrder = new Order { UserId = userId };
        var request = new DeleteOrderRequest() { OrderId = TestConstants.OrderId };
        
        _mockAuthService
            .Setup(x => x.IsAdminAsync(It.IsAny<long>()))
            .ReturnsAsync(true);
        _mockOrderRepository
            .Setup(x => x.GetOrderById(It.IsAny<long>(), It.IsAny<bool>()))
            .ReturnsAsync(mockOrder);

        // Act
        var result = await _orderService.DeleteOrder(request, _defaultServerCallContext);

        // Assert
        result.Should().BeEquivalentTo(new Empty());
        _mockOrderRepository.Verify(x => x.GetOrderById(request.OrderId, It.IsAny<bool>()), Times.Once);
        _mockOrderRepository.Verify(x => x.DeleteOrderById(It.IsAny<long>()), Times.Once);
        _mockOrderItemRepository.Verify(x => x.DeleteAllOrderItemsByOrderId(It.IsAny<long>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CreateOrder_OrderIsCreatedSuccessfully_ReturnOrderDto()
    {
        // Arrange
        var beerId = 1;
        var request = new CreateOrderRequest
        {
            Order = new CreateOrderDto() { Items = { new CreateOrderItemDto { BeerId = beerId } }}
        };
        List<Beer> beers = [new() { BeerId = beerId, }];
        
        _mockCatalogService
            .Setup(x => x.GetBeersByIdAsync(It.IsAny<IEnumerable<CreateOrderItemDto>>()))
            .ReturnsAsync(beers);

        // Act
        var result = await _orderService.CreateOrder(request, _defaultServerCallContext);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(TestConstants.UserId);
        result.Status.Should().Be(StatusOrderDto.Created);
        result.Items.Should().AllSatisfy(item => item.BeerId.Should().Be(beerId));
        _mockOrderRepository.Verify(x => x.CreateOrder(It.IsAny<Order>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitChangesAsync(), Times.Once);
    }
}