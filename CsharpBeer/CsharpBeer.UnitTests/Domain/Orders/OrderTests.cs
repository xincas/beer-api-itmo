using CsharpBeer.CommonTests;
using CsharpBeer.OrderService.Domain.Orders;
using FluentAssertions;

namespace CsharpBeer.UnitTests.Domain.Orders;

public class OrderTests
{
    [Fact]
    public void AddOrderItem_WhenOrderItemWithThisIdAlreadyExists_ShouldDoNothing()
    {
        //Arrange
        var order = OrderFactory.Create();
        var orderItem1Price = 100d;
        var orderItem1Quantity = 2;
        var orderItem1 = OrderItemFactory.Create(price: orderItem1Price, quantity: orderItem1Quantity);
        var orderItem2 = OrderItemFactory.Create();
        
        //Act
        order.AddItem(orderItem1);
        order.AddItem(orderItem2);

        //Assertion
        order.Items.Count.Should().Be(1);
        order.Items.First().Should().BeEquivalentTo(new OrderItem
        {
            Price = orderItem1Price,
            Quantity = orderItem1Quantity,
        });
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void AddOrderItem_WhenAddNewItem_ShouldDefineOrderIdInOrderItem(int orderId)
    {
        //Arrange
        var order = OrderFactory.Create();
        order.OrderId = orderId;
        var orderItem1 = OrderItemFactory.Create();
        
        //Act
        order.AddItem(orderItem1);

        //Assertion
        order.Items.First().OrderId.Should().Be(orderId);
    }
    
    [Theory]
    [InlineData(50d, 2, 100)]
    [InlineData(50d, 0, 0)]
    [InlineData(12.5d, 2, 25)]
    public void AddOrderItem_WhenAddNewItem_ShouldRecalculateTotal(double beerPrice, int quantity, double total)
    {
        //Arrange
        var order = OrderFactory.Create();
        var orderItem1 = OrderItemFactory.Create(quantity: quantity, price: beerPrice);
        
        //Act
        order.AddItem(orderItem1);

        //Assertion
        order.Total.Should().Be(total);
    }
    
    [Fact]
    public void AddOrderItems_WhenTwoItemsWithSameId_AddOnlyFirst()
    {
        //Arrange
        var order = OrderFactory.Create();
        var orderItem1Price = 100d;
        var orderItem1Quantity = 2;
        
        var orderItem1 = OrderItemFactory.Create(price: orderItem1Price, quantity: orderItem1Quantity);
        var orderItem2 = OrderItemFactory.Create();
        
        //Act
        order.AddItems([orderItem1, orderItem2]);

        //Assertion
        order.Items.Count.Should().Be(1);
        order.Items.First().Should().BeEquivalentTo(new OrderItem
        {
            Price = orderItem1Price,
            Quantity = orderItem1Quantity,
        });
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void AddOrderItems_WhenAddNewItems_ShouldDefineOrderIdInOrderItem(int orderId)
    {
        //Arrange
        var order = OrderFactory.Create();
        order.OrderId = orderId;
        var orderItem1 = OrderItemFactory.Create();
        var orderItem2 = OrderItemFactory.Create(beerId: 1);
        
        //Act
        order.AddItems([orderItem1, orderItem2]);

        //Assertion
        order.Items.Should().AllSatisfy(item => item.OrderId.Should().Be(orderId));
    }
    
    [Theory]
    [InlineData(50d, 2, 200)]
    [InlineData(50d, 0, 0)]
    [InlineData(12.5d, 2, 50)]
    public void AddOrderItems_WhenAddNewItems_ShouldRecalculateTotal(double beerPrice, int quantity, double total)
    {
        //Arrange
        var order = OrderFactory.Create();

        var orderItem1 = OrderItemFactory.Create(quantity: quantity, price: beerPrice);
        var orderItem2 = OrderItemFactory.Create(beerId: 1, quantity: quantity, price: beerPrice);
        
        //Act
        order.AddItems([orderItem1, orderItem2]);

        //Assertion
        order.Total.Should().Be(total);
    }
    
    [Fact]
    public void Create_WhenOrderItemsPresented_ShouldCreateCorrectOrder()
    {
        //Arrange
        const long userId = 1;
        var orderItem = OrderItemFactory.Create();
        
        //Act
        var order = Order.Create(userId: userId, [orderItem]);

        //Assertion
        order.UserId.Should().Be(userId);
        order.Total.Should().Be(orderItem.Price * orderItem.Quantity);
        order.Items.Count.Should().Be(1);
        order.Items.Should()
            .AllSatisfy(item => item.Should()
                .BeEquivalentTo(OrderItemFactory.DefaultOrderItemEquivalent));
    }
}