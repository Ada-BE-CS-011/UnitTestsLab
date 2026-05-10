using OrdersApi.Good.Application;
using OrdersApi.Good.Application.Services;
using OrdersApi.Good.Domain.Interfaces;
using OrdersApi.Good.Dtos;
using OrdersApi.Good.Infrastructure.Repositories;

namespace OrdersApi.Tests.Application.Services;

public class OrderServiceTests_GetOrderById
{
    [Fact]
    public void GetOrderById_DeveRetornarNull_QuandoPedidoNaoExiste()
    {
        var service = CreateService();

        var result = service.GetOrderById(999);

        Assert.Null(result);
    }

    [Fact]
    public void GetOrderById_DeveRetornarPedido_QuandoPedidoExiste()
    {
        var service = CreateService();
        var createResult = service.CreateOrder(new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        });

        var result = service.GetOrderById(createResult.Order!.Id);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetOrderById_DeveRetornarPedidoCorreto()
    {
        var service = CreateService();
        var createResult = service.CreateOrder(new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        });

        var result = service.GetOrderById(createResult.Order!.Id);

        Assert.Equal(createResult.Order.Id, result!.Id);
    }

    [Fact]
    public void GetOrderById_DeveRetornarTodosOsDados()
    {
        var service = CreateService();
        var createResult = service.CreateOrder(new CreateOrderRequestDto
        {
            Coupon = "DESC10",
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        });

        var result = service.GetOrderById(createResult.Order!.Id);

        Assert.Equal(createResult.Order.Id, result!.Id);
        Assert.Equal(createResult.Order.Status, result.Status);
        Assert.Equal(createResult.Order.Subtotal, result.Subtotal);
        Assert.Equal(createResult.Order.Discount, result.Discount);
        Assert.Equal(createResult.Order.Shipping, result.Shipping);
        Assert.Equal(createResult.Order.Total, result.Total);
        Assert.Equal(createResult.Order.Coupon, result.Coupon);
        Assert.Equal(createResult.Order.CreatedAt, result.CreatedAt);
        Assert.Equal(createResult.Order.Items.Count, result.Items.Count);
    }

    private static OrderService CreateService()
    {
        return new OrderService(
            new InMemoryProductRepository(),
            new InMemoryOrderRepository(),
            new FakePaymentGateway(),
            new FixedDateTimeProvider(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
    }

    private class FakePaymentGateway : IPaymentGateway
    {
        public bool Process(decimal amount) => amount <= 1000m;
    }

    private class FixedDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTime _utcNow;
        public FixedDateTimeProvider(DateTime utcNow) => _utcNow = utcNow;
        public DateTime UtcNow => _utcNow;
    }
}
