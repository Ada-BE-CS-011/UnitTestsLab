using OrdersApi.Good.Domain.Entities;
using OrdersApi.Good.Domain.Interfaces;

namespace OrdersApi.Good.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products =
    [
        new Product { Id = 1, Name = "Notebook", Price = 3500m, Stock = 5 },
        new Product { Id = 2, Name = "Mouse", Price = 80m, Stock = 20 },
        new Product { Id = 3, Name = "Teclado", Price = 150m, Stock = 10 },
        new Product { Id = 4, Name = "Monitor", Price = 900m, Stock = 8 }
    ];

    public IReadOnlyList<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public void Update(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index >= 0)
        {
            _products[index] = product;
        }
    }
}
