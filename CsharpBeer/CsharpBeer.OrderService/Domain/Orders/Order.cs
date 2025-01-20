using CsharpBeer.OrderService.Domain.Common.Extensions.Orders;

namespace CsharpBeer.OrderService.Domain.Orders;

public class Order
{
    private List<OrderItem> _items = [];
    public long OrderId { get; set; }
    public long UserId { get; set; }
    public double Total { get; set; }
    public OrderStatus Status { get; set; }
    public IReadOnlyCollection<OrderItem> Items => _items;

    public void AddItem(OrderItem orderItem)
    {
        var item = _items.FirstOrDefault(oi => oi.BeerId == orderItem.BeerId);
        if (item is not null) return;

        orderItem.OrderId = OrderId;
        _items.Add(orderItem);
        Total = _items.Sum(oi => oi.Quantity * oi.Price);
    }

    public void AddItems(IEnumerable<OrderItem> items)
    {
        foreach (var orderItem in items) AddItem(orderItem);
    }

    private bool UpdateItem(OrderItem orderItem)
    {
        var item = _items.FirstOrDefault(oi => oi.BeerId == orderItem.BeerId);
        if (item is null) return false;

        var willTotalChange = item.WillTotalChange(orderItem);
        item.CopyFieldsIfNotNull(orderItem);
        Total = willTotalChange ? _items.Sum(oi => oi.Quantity * oi.Price) : Total;
        return true;
    }

    public void UpdateOrAddItem(OrderItem orderItem)
    {
        if (!UpdateItem(orderItem))
        {
            AddItem(orderItem);
        }
    }
    
    public void UpdateOrAddItems(IEnumerable<OrderItem> orderItems)
    {
        foreach (var item in orderItems) UpdateOrAddItem(item);
    }
    
    public static Order Create(long userId, IEnumerable<OrderItem> items)
    {
        var orderItems = items.ToList();
        var order = new Order()
        {
            OrderId = default,
            UserId = userId,
            Status = OrderStatus.Created,
        };
        order.AddItems(orderItems);
        return order;
    }
}