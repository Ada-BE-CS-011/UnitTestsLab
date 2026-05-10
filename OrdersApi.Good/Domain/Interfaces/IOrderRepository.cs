using OrdersApi.Good.Domain.Entities;

namespace OrdersApi.Good.Domain.Interfaces;

public interface IOrderRepository
{
    Order Add(Order order);
    Order? GetById(int id);
    List<Order> GetAll();
}
