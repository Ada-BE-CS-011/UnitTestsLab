using OrdersApi.Good.Domain.Entities;

namespace OrdersApi.Good.Domain.Interfaces;

public interface IProductRepository
{
    IReadOnlyList<Product> GetAll();
    Product? GetById(int id);
    void Update(Product product);
}
