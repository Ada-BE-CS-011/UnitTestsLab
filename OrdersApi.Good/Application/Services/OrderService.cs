using OrdersApi.Good.Application.Interfaces;
using OrdersApi.Good.Application.Models;
using OrdersApi.Good.Domain.Entities;
using OrdersApi.Good.Domain.Interfaces;
using OrdersApi.Good.Dtos;

namespace OrdersApi.Good.Application.Services;

public class OrderService : IOrderService
{
    private const decimal Desc10DiscountRate = 0.10m;
    private const decimal Vip20DiscountRate = 0.20m;
    private const decimal DefaultShipping = 20m;
    private const decimal FreeShippingThreshold = 300m;
    private const decimal Vip20Threshold = 500m;

    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OrderService(
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IPaymentGateway paymentGateway,
        IDateTimeProvider dateTimeProvider)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _paymentGateway = paymentGateway;
        _dateTimeProvider = dateTimeProvider;
    }

    public OrderOperationResult CreateOrder(CreateOrderRequestDto request)
    {
        if (request.Items == null || request.Items.Count == 0)
        {
            return Fail("Um pedido deve ter pelo menos 1 item.");
        }

        var order = new Order
        {
            Coupon = request.Coupon,
            Status = OrderStatus.Pending,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        foreach (var itemDto in request.Items)
        {
            if (itemDto.Quantity <= 0)
            {
                return Fail("Quantity deve ser maior que zero.");
            }

            var product = _productRepository.GetById(itemDto.ProductId);
            if (product is null)
            {
                return Fail($"Produto {itemDto.ProductId} não existe.");
            }

            if (itemDto.Quantity > product.Stock)
            {
                return Fail($"Estoque insuficiente para produto {product.Name}.");
            }

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });
        }

        order.Subtotal = order.Items.Sum(i => i.LineTotal);
        order.Discount = CalculateDiscount(order.Subtotal, order.Coupon);
        order.Shipping = CalculateShipping(order.Subtotal);
        order.Total = order.Subtotal - order.Discount + order.Shipping;

        var paymentApproved = _paymentGateway.Process(order.Total);
        if (paymentApproved)
        {
            order.Status = OrderStatus.Approved;
            DecrementStock(order.Items);
        }
        else
        {
            order.Status = OrderStatus.Rejected;
        }

        var saved = _orderRepository.Add(order);

        return new OrderOperationResult
        {
            Success = true,
            Message = order.Status == OrderStatus.Approved ? "Pedido aprovado." : "Pedido recusado.",
            Order = ToResponse(saved)
        };
    }

    public OrderResponseDto? GetOrderById(int id)
    {
        var order = _orderRepository.GetById(id);
        return order is null ? null : ToResponse(order);
    }

    public List<OrderResponseDto> GetAllOrders()
    {
        var orders = _orderRepository.GetAll();
        return orders.Select(ToResponse).ToList();
    }

    private decimal CalculateDiscount(decimal subtotal, string? coupon)
    {
        if (string.Equals(coupon, "DESC10", StringComparison.OrdinalIgnoreCase))
        {
            return subtotal * Desc10DiscountRate;
        }

        if (string.Equals(coupon, "VIP20", StringComparison.OrdinalIgnoreCase) && subtotal > Vip20Threshold)
        {
            return subtotal * Vip20DiscountRate;
        }

        return 0m;
    }

    private static decimal CalculateShipping(decimal subtotal)
    {
        return subtotal > FreeShippingThreshold ? 0m : DefaultShipping;
    }

    private void DecrementStock(IEnumerable<OrderItem> items)
    {
        foreach (var item in items)
        {
            var product = _productRepository.GetById(item.ProductId);
            if (product is null)
            {
                continue;
            }

            product.Stock -= item.Quantity;
            _productRepository.Update(product);
        }
    }

    private static OrderOperationResult Fail(string message)
    {
        return new OrderOperationResult
        {
            Success = false,
            Message = message
        };
    }

    private static OrderResponseDto ToResponse(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            Status = order.Status.ToString(),
            Coupon = order.Coupon,
            Subtotal = order.Subtotal,
            Discount = order.Discount,
            Shipping = order.Shipping,
            Total = order.Total,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}
