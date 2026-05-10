using OrdersApi.Bad.Domain.Entities;

namespace OrdersApi.Bad.Infrastructure.Repositories;

public class OrderRepository
{
    private static readonly List<Order> _orders = new();
    private static int _nextId = 1;

    public Order Add(Order order)
    {
        order.Id = _nextId;
        _nextId++;
        _orders.Add(order);
        return order;
    }

    public Order? GetById(int id)
    {
        return _orders.FirstOrDefault(x => x.Id == id);
    }

    public List<Order> GetAll()
    {
        return _orders;
    }
}
