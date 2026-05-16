using Microsoft.EntityFrameworkCore;
using OrdersApi.Bad.Domain.Entities;
using OrdersApi.Bad.Domain.Interfaces;
using OrdersApi.Bad.Infrastructure.Database;

namespace OrdersApi.Bad.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext context;

    public OrderRepository(AppDbContext context)
    {
        this.context = context;
    }

    public Order Add(Order order)
    {
        context.Orders.Add(order);
        context.SaveChanges();

        return order;
    }

    public Order? GetById(int id)
    {
        return context.Orders
            .Include(o => o.Items)
            .FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<Order> GetAll()
    {
        return context.Orders
            .Include(o => o.Items)
            .AsNoTracking();
    }
}
