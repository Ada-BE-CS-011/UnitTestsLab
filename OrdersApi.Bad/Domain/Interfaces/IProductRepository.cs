using OrdersApi.Bad.Domain.Entities;

namespace OrdersApi.Bad.Domain.Interfaces
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product? GetById(int id);
    }
}