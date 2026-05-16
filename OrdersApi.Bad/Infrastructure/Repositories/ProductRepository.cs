using Microsoft.EntityFrameworkCore;
using OrdersApi.Bad.Domain.Entities;
using OrdersApi.Bad.Infrastructure.Database;

namespace OrdersApi.Bad.Infrastructure.Repositories;

public class ProductRepository
{
    //private static readonly List<Product> _products =
    //[
    //    new Product { Id = 1, Name = "Notebook", Price = 3500m, Stock = 5 },
    //    new Product { Id = 2, Name = "Mouse", Price = 80m, Stock = 20 },
    //    new Product { Id = 3, Name = "Teclado", Price = 150m, Stock = 10 },
    //    new Product { Id = 4, Name = "Monitor", Price = 900m, Stock = 8 }
    //];
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
