using OrdersApi.Good.Application;
using OrdersApi.Good.Application.Services;
using OrdersApi.Good.Domain.Interfaces;
using OrdersApi.Good.Dtos;
using OrdersApi.Good.Infrastructure.Repositories;

namespace OrdersApi.Tests.Application.Services;

public class OrderServiceTests_CreateOrder_OrderCalculation
{
    [Fact]
    public void CreateOrder_DeveCalcularSubtotalCorretamente_ComUmItem()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 3 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(240m, result.Order!.Subtotal);
    }

    [Fact]
    public void CreateOrder_DeveCalcularSubtotalCorretamente_ComMultiplosItens()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [
                new CreateOrderItemDto { ProductId = 2, Quantity = 2 },
                new CreateOrderItemDto { ProductId = 3, Quantity = 1 }
            ]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(310m, result.Order!.Subtotal);
    }

    [Fact]
    public void CreateOrder_DeveCalcularTotalCorretamente_SemDescontoESemFreteGratis()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(80m, result.Order!.Subtotal);
        Assert.Equal(0m, result.Order.Discount);
        Assert.Equal(20m, result.Order.Shipping);
        Assert.Equal(100m, result.Order.Total);
    }

    [Fact]
    public void CreateOrder_DeveCalcularTotalCorretamente_ComDescontoEFreteGratis()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Coupon = "VIP20",
            Items = [new CreateOrderItemDto { ProductId = 1, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(3500m, result.Order!.Subtotal);
        Assert.Equal(700m, result.Order.Discount);
        Assert.Equal(0m, result.Order.Shipping);
        Assert.Equal(2800m, result.Order.Total);
    }

    [Fact]
    public void CreateOrder_DevePreencherItensCorretamente()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 3 }]
        };

        var result = service.CreateOrder(request);

        Assert.Single(result.Order!.Items);
        Assert.Equal(2, result.Order.Items[0].ProductId);
        Assert.Equal("Mouse", result.Order.Items[0].ProductName);
        Assert.Equal(3, result.Order.Items[0].Quantity);
        Assert.Equal(80m, result.Order.Items[0].UnitPrice);
    }

    [Fact]
    public void CreateOrder_DevePreencherMultiplosItensCorretamente()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [
                new CreateOrderItemDto { ProductId = 2, Quantity = 1 },
                new CreateOrderItemDto { ProductId = 3, Quantity = 2 }
            ]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(2, result.Order!.Items.Count);
    }

    [Fact]
    public void CreateOrder_DevePreencherCupomCorretamente()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Coupon = "DESC10",
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal("DESC10", result.Order!.Coupon);
    }

    [Fact]
    public void CreateOrder_DeveUsarDataDoProvider()
    {
        var fixedDate = new DateTime(2026, 5, 10, 14, 30, 0, DateTimeKind.Utc);
        var service = CreateService(dateTimeProvider: new FixedDateTimeProvider(fixedDate));
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(fixedDate, result.Order!.CreatedAt);
    }

    private static OrderService CreateService(IDateTimeProvider? dateTimeProvider = null)
    {
        return new OrderService(
            new InMemoryProductRepository(),
            new InMemoryOrderRepository(),
            new FakePaymentGateway(),
            dateTimeProvider ?? new FixedDateTimeProvider(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
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
