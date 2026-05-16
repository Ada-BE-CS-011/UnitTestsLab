using ErrorOr;
using OrdersApi.Bad.Domain.Entities;
using OrdersApi.Bad.Dtos;

namespace OrdersApi.Bad.Application.Services
{
    public interface IOrderService
    {
        ErrorOr<Order?> CreateOrder(CreateOrderRequest request);
    }
}