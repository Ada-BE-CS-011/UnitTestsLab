namespace OrdersApi.Good.Dtos;

public class CreateOrderRequestDto
{
    public List<CreateOrderItemDto> Items { get; set; } = new();
    public string? Coupon { get; set; }
}

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
