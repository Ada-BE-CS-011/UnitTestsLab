using Microsoft.EntityFrameworkCore;
using OrdersApi.Bad.Domain.Entities;
using OrdersApi.Bad.Domain.Interfaces;
using OrdersApi.Bad.Infrastructure.Database;

namespace OrdersApi.Bad.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private AppDbContext dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IEnumerable<Product> GetAll()
    {
        return dbContext.Products.AsNoTracking();
    }

    public Product? GetById(int id)
    {
        return dbContext.Products.FirstOrDefault(x => x.Id == id);
    }
}
