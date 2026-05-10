using OrdersApi.Good.Application;
using OrdersApi.Good.Application.Services;
using OrdersApi.Good.Domain.Interfaces;
using OrdersApi.Good.Dtos;
using OrdersApi.Good.Infrastructure.Repositories;

namespace OrdersApi.Tests.Application.Services;

public class OrderServiceTests_CreateOrder_Shipping
{
    [Fact]
    public void CreateOrder_DeveCobrarFrete20_QuandoSubtotalMenorOuIgualA300()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(20m, result.Order!.Shipping);
    }

    [Fact]
    public void CreateOrder_DeveCobrarFrete20_QuandoSubtotalExatamente300()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 3, Quantity = 2 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(300m, result.Order!.Subtotal);
        Assert.Equal(20m, result.Order.Shipping);
    }

    [Fact]
    public void CreateOrder_DeveAplicarFreteGratis_QuandoSubtotalMaiorQue300()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 3, Quantity = 3 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal(450m, result.Order!.Subtotal);
        Assert.Equal(0m, result.Order.Shipping);
    }

    [Fact]
    public void CreateOrder_DeveAplicarFreteGratis_QuandoSubtotalExatamente301()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [
                new CreateOrderItemDto { ProductId = 3, Quantity = 2 },
                new CreateOrderItemDto { ProductId = 2, Quantity = 1 }
            ]
        };

        var result = service.CreateOrder(request);

        Assert.True(result.Order!.Subtotal > 300m);
        Assert.Equal(0m, result.Order.Shipping);
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
