using CsharpBeer.OrderService.Domain.Orders;

namespace CsharpBeer.CommonTests;

public static class OrderFactory
{
    public static Order Create(
        long? userId = null,
        IEnumerable<OrderItem>? orderItems = null) => 
        Order.Create(
            userId ?? 0, 
            orderItems ?? []);
    
    public static Order DefaultOrderEquivalent = Order.Create(0, []);
}