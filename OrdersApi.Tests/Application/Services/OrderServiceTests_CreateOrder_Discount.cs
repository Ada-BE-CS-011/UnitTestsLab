using OrdersApi.Good.Application;
using OrdersApi.Good.Application.Services;
using OrdersApi.Good.Domain.Interfaces;
using OrdersApi.Good.Dtos;
using OrdersApi.Good.Infrastructure.Repositories;

namespace OrdersApi.Tests.Application.Services;

public class OrderServiceTests_CreateOrder_Discount
{
    [Fact]
    public void CreateOrder_DeveAplicarDesconto10Porcento_QuandoCupomDESC10()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Coupon = "DESC10",
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(8m, result.Order!.Discount);
    }

    [Fact]
    public void CreateOrder_DeveCalcularTotalCorretamente_QuandoCupomDESC10()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Coupon = "DESC10",
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(80m, result.Order!.Subtotal);
        Assert.Equal(8m, result.Order.Discount);
        Assert.Equal(20m, result.Order.Shipping);
        Assert.Equal(92m, result.Order.Total);
    }

    [Fact]
    public void CreateOrder_DeveFuncionarComCupomDESC10_EmMinusculas()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Coupon = "desc10",
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(8m, result.Order!.Discount);
    }

    [Fact]
    public void CreateOrder_NaoDeveAplicarDesconto_QuandoCupomVIP20_ESubtotalMenorQue500()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Coupon = "VIP20",
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(0m, result.Order!.Discount);
    }

    [Fact]
    public void CreateOrder_DeveAplicarDesconto20Porcento_QuandoCupomVIP20_ESubtotalMaiorQue500()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Coupon = "VIP20",
            Items = [new CreateOrderItemDto { ProductId = 4, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(180m, result.Order!.Discount);
    }

    [Fact]
    public void CreateOrder_DeveCalcularTotalCorretamente_QuandoCupomVIP20_ESubtotalMaiorQue500()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Coupon = "VIP20",
            Items = [new CreateOrderItemDto { ProductId = 4, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(900m, result.Order!.Subtotal);
        Assert.Equal(180m, result.Order.Discount);
        Assert.Equal(0m, result.Order.Shipping);
        Assert.Equal(720m, result.Order.Total);
    }

    [Fact]
    public void CreateOrder_NaoDeveAplicarDesconto_QuandoCupomInvalido()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Coupon = "INVALIDO",
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(0m, result.Order!.Discount);
    }

    [Fact]
    public void CreateOrder_NaoDeveAplicarDesconto_QuandoCupomNulo()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Coupon = null,
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(0m, result.Order!.Discount);
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
