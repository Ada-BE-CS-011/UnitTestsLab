namespace OrdersApi.Good.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public string? Coupon { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Shipping { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; }
}
