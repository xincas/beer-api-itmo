﻿using CsharpBeer.OrderService.Domain.Orders;

namespace CsharpBeer.OrderService.Domain.Common.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetOrderById(long id, bool includeItems = false);
    Task<Order?> GetOrderByIdAsNoTrack(long id);
    Task<List<Order>> ListOrdersByUserId(long userId);
    Task<List<Order>> ListOrdersByUserIdAsNoTrack(long userId);
    Task<Order> CreateOrder(Order order);
    Task<Order> UpdateOrder(Order order);
    Task DeleteOrderById(long id);
}