using CsharpBeer.OrderService.Domain.Orders;

namespace CsharpBeer.CommonTests;

public static class OrderItemFactory
{
    public static OrderItem Create(
        long? beerId = null,
        int? quantity = null,
        double? price = null) =>
        OrderItem.Create(
            beerId ?? 0,
            quantity ?? 1,
            price ?? 20d);

    public static OrderItem DefaultOrderItemEquivalent = OrderItem.Create(0, 1, 20d);
}