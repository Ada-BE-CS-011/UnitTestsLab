using OrdersApi.Bad.Domain.Entities;

namespace OrdersApi.Bad.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Order Add(Order order);
        IEnumerable<Order> GetAll();
        Order? GetById(int id);
    }
}