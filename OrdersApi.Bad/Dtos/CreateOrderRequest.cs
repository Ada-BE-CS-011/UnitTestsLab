namespace OrdersApi.Bad.Dtos;

public class CreateOrderRequest
{
    public List<CreateOrderItemRequest>? Items { get; set; }
    public string? Coupon { get; set; }
}
