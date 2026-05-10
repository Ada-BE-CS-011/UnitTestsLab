using OrdersApi.Good.Application.Models;
using OrdersApi.Good.Dtos;

namespace OrdersApi.Good.Application.Interfaces;

public interface IOrderService
{
    OrderOperationResult CreateOrder(CreateOrderRequestDto request);
    List<OrderResponseDto> GetAllOrders();
    OrderResponseDto? GetOrderById(int id);
}
