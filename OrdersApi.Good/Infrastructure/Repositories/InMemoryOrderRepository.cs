using OrdersApi.Good.Domain.Entities;
using OrdersApi.Good.Domain.Interfaces;

namespace OrdersApi.Good.Infrastructure.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();
    private int _nextId = 1;

    public Order Add(Order order)
    {
        order.Id = _nextId++;
        _orders.Add(order);
        return order;
    }

    public Order? GetById(int id)
    {
        return _orders.FirstOrDefault(o => o.Id == id);
    }

    public List<Order> GetAll()
    {
        return _orders;
    }
}
