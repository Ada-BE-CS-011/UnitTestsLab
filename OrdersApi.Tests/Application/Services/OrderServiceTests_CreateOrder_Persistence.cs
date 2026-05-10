using OrdersApi.Good.Application;
using OrdersApi.Good.Application.Services;
using OrdersApi.Good.Domain.Interfaces;
using OrdersApi.Good.Dtos;
using OrdersApi.Good.Infrastructure.Repositories;

namespace OrdersApi.Tests.Application.Services;

public class OrderServiceTests_CreateOrder_Persistence
{
    [Fact]
    public void CreateOrder_DeveGerarIdsSequenciais()
    {
        var service = CreateService();

        var result1 = service.CreateOrder(new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        });

        var result2 = service.CreateOrder(new CreateOrderRequestDto
        {
            Items = [new CreateOrderItemDto { ProductId = 2, Quantity = 1 }]
        });

        Assert.Equal(result1.Order!.Id + 1, result2.Order!.Id);
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
