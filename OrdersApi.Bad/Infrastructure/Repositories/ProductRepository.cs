using OrdersApi.Bad.Domain.Entities;

namespace OrdersApi.Bad.Infrastructure.Repositories;

public class ProductRepository
{
    private static readonly List<Product> _products =
    [
        new Product { Id = 1, Name = "Notebook", Price = 3500m, Stock = 5 },
        new Product { Id = 2, Name = "Mouse", Price = 80m, Stock = 20 },
        new Product { Id = 3, Name = "Teclado", Price = 150m, Stock = 10 },
        new Product { Id = 4, Name = "Monitor", Price = 900m, Stock = 8 }
    ];

    public List<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(int id)
    {
        return _products.FirstOrDefault(x => x.Id == id);
    }
}
