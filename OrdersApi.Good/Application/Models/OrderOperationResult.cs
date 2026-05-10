using OrdersApi.Good.Dtos;

namespace OrdersApi.Good.Application.Models;

public class OrderOperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public OrderResponseDto? Order { get; set; }
}
