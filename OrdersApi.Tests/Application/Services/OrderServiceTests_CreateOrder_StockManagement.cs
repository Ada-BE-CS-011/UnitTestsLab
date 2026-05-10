using OrdersApi.Good.Application;
using OrdersApi.Good.Application.Services;
using OrdersApi.Good.Domain.Interfaces;
using OrdersApi.Good.Dtos;
using OrdersApi.Good.Infrastructure.Repositories;

namespace OrdersApi.Tests.Application.Services;

public class OrderServiceTests_CreateOrder_StockManagement
{
    [Fact]
    public void CreateOrder_DeveDecrementarEstoque_QuandoPagamentoAprovado()
    {
        var productRepository = new InMemoryProductRepository();
        var service = CreateService(productRepository: productRepository);
        var stockBefore = productRepository.GetById(2)!.Stock;

        service.CreateOrder(new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 3 }]
        });

        var stockAfter = productRepository.GetById(2)!.Stock;
        Assert.Equal(stockBefore - 3, stockAfter);
    }

    [Fact]
    public void CreateOrder_NaoDeveDecrementarEstoque_QuandoPagamentoRecusado()
    {
        var productRepository = new InMemoryProductRepository();
        var service = CreateService(productRepository: productRepository);
        var stockBefore = productRepository.GetById(1)!.Stock;

        service.CreateOrder(new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 1, Quantity = 1 }]
        });

        var stockAfter = productRepository.GetById(1)!.Stock;
        Assert.Equal(stockBefore, stockAfter);
    }

    [Fact]
    public void CreateOrder_DeveDecrementarEstoqueCorretamente_QuandoMultiplosItens()
    {
        var productRepository = new InMemoryProductRepository();
        var service = CreateService(productRepository: productRepository);
        var stock2Before = productRepository.GetById(2)!.Stock;
        var stock3Before = productRepository.GetById(3)!.Stock;

        service.CreateOrder(new CreateOrderRequestDto
        {
            Items = [
                new CreateOrderItemDto { ProductId = 2, Quantity = 2 },
                new CreateOrderItemDto { ProductId = 3, Quantity = 1 }
            ]
        });

        var stock2After = productRepository.GetById(2)!.Stock;
        var stock3After = productRepository.GetById(3)!.Stock;
        Assert.Equal(stock2Before - 2, stock2After);
        Assert.Equal(stock3Before - 1, stock3After);
    }

    private static OrderService CreateService(IProductRepository? productRepository = null)
    {
        return new OrderService(
            productRepository ?? new InMemoryProductRepository(),
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
