using OrdersApi.Good.Application;
using OrdersApi.Good.Application.Services;
using OrdersApi.Good.Domain.Entities;
using OrdersApi.Good.Domain.Interfaces;
using OrdersApi.Good.Dtos;
using OrdersApi.Good.Infrastructure.Repositories;

namespace OrdersApi.Tests.Application.Services;

public class OrderServiceTests_CreateOrder_Payment
{
    [Fact]
    public void CreateOrder_DeveAprovarPedido_QuandoTotalMenorOuIgualA1000()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Equal("Approved", result.Order!.Status);
    }

    [Fact]
    public void CreateOrder_DeveAprovarPedido_QuandoTotalExatamente1000()
    {
        var paymentGateway = new SpyPaymentGateway();
        var service = CreateService(paymentGateway: paymentGateway);
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 4, Quantity = 1 }],
            Coupon = "DESC10"
        };

        var result = service.CreateOrder(request);

        Assert.True(paymentGateway.LastAmount <= 1000m);
        Assert.Equal("Approved", result.Order!.Status);
    }

    [Fact]
    public void CreateOrder_DeveRejeitarPedido_QuandoTotalMaiorQue1000()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 1, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.True(result.Order!.Total > 1000m);
        Assert.Equal("Rejected", result.Order.Status);
    }

    [Fact]
    public void CreateOrder_DeveRetornarSucesso_MesmoQuandoPedidoRejeitado()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 1, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.True(result.Success);
        Assert.Equal("Rejected", result.Order!.Status);
    }

    [Fact]
    public void CreateOrder_DeveTerMensagemAprovado_QuandoPagamentoAprovado()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Contains("aprovado", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateOrder_DeveTerMensagemRecusado_QuandoPagamentoRecusado()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 1, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Contains("recusado", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateOrder_DeveChamarGatewayComValorCorreto()
    {
        var paymentGateway = new SpyPaymentGateway();
        var service = CreateService(paymentGateway: paymentGateway);
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        service.CreateOrder(request);

        Assert.Equal(100m, paymentGateway.LastAmount);
    }

    [Fact]
    public void CreateOrder_DeveChamarGatewayApenas1Vez()
    {
        var paymentGateway = new SpyPaymentGateway();
        var service = CreateService(paymentGateway: paymentGateway);
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        };

        service.CreateOrder(request);

        Assert.Equal(1, paymentGateway.CallCount);
    }

    private static OrderService CreateService(IPaymentGateway? paymentGateway = null)
    {
        return new OrderService(
            new InMemoryProductRepository(),
            new InMemoryOrderRepository(),
            paymentGateway ?? new FakePaymentGateway(),
            new FixedDateTimeProvider(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
    }

    private class FakePaymentGateway : IPaymentGateway
    {
        public bool Process(decimal amount) => amount <= 1000m;
    }

    private class SpyPaymentGateway : IPaymentGateway
    {
        public decimal LastAmount { get; private set; }
        public int CallCount { get; private set; }

        public bool Process(decimal amount)
        {
            CallCount++;
            LastAmount = amount;
            return amount <= 1000m;
        }
    }

    private class FixedDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTime _utcNow;
        public FixedDateTimeProvider(DateTime utcNow) => _utcNow = utcNow;
        public DateTime UtcNow => _utcNow;
    }
}
