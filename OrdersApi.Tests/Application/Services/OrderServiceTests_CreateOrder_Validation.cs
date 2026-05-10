using OrdersApi.Good.Application;
using OrdersApi.Good.Application.Services;
using OrdersApi.Good.Domain.Interfaces;
using OrdersApi.Good.Dtos;
using OrdersApi.Good.Infrastructure.Repositories;

namespace OrdersApi.Tests.Application.Services;

public class OrderServiceTests_CreateOrder_Validation
{
    [Fact]
    public void CreateOrder_DeveRetornarErro_QuandoPedidoNaoTemItens()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto { Items = [] };

        var result = service.CreateOrder(request);

        Assert.False(result.Success);
    }

    [Fact]
    public void CreateOrder_DeveTerMensagemEspecifica_QuandoPedidoNaoTemItens()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto { Items = [] };

        var result = service.CreateOrder(request);

        Assert.Contains("pelo menos 1 item", result.Message);
    }

    [Fact]
    public void CreateOrder_NaoDeveRetornarOrder_QuandoPedidoNaoTemItens()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto { Items = [] };

        var result = service.CreateOrder(request);

        Assert.Null(result.Order);
    }

    [Fact]
    public void CreateOrder_DeveRetornarErro_QuandoQuantidadeEZero()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 1, Quantity = 0 }]
        };

        var result = service.CreateOrder(request);

        Assert.False(result.Success);
    }

    [Fact]
    public void CreateOrder_DeveTerMensagemEspecifica_QuandoQuantidadeEZero()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 1, Quantity = 0 }]
        };

        var result = service.CreateOrder(request);

        Assert.Contains("maior que zero", result.Message);
    }

    [Fact]
    public void CreateOrder_DeveRetornarErro_QuandoQuantidadeENegativa()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 1, Quantity = -5 }]
        };

        var result = service.CreateOrder(request);

        Assert.False(result.Success);
    }

    [Fact]
    public void CreateOrder_DeveRetornarErro_QuandoProdutoNaoExiste()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 999, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.False(result.Success);
    }

    [Fact]
    public void CreateOrder_DeveTerMensagemEspecifica_QuandoProdutoNaoExiste()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 999, Quantity = 1 }]
        };

        var result = service.CreateOrder(request);

        Assert.Contains("não existe", result.Message);
    }

    [Fact]
    public void CreateOrder_DeveRetornarErro_QuandoEstoqueInsuficiente()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 1, Quantity = 999 }]
        };

        var result = service.CreateOrder(request);

        Assert.False(result.Success);
    }

    [Fact]
    public void CreateOrder_DeveTerMensagemEspecifica_QuandoEstoqueInsuficiente()
    {
        var service = CreateService();
        var request = new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 1, Quantity = 999 }]
        };

        var result = service.CreateOrder(request);

        Assert.Contains("Estoque insuficiente", result.Message);
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
