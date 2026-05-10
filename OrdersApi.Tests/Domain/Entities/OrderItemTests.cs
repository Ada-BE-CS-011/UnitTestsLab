using OrdersApi.Good.Domain.Entities;

namespace OrdersApi.Tests.Domain.Entities;

public class OrderItemTests
{
    [Fact]
    public void LineTotal_DeveCalcularCorretamente()
    {
        var item = new OrderItem
        {
            UnitPrice = 10m,
            Quantity = 3
        };

        var lineTotal = item.LineTotal;

        Assert.Equal(30m, lineTotal);
    }

    [Fact]
    public void LineTotal_DeveRetornarZero_QuandoQuantidadeZero()
    {
        var item = new OrderItem
        {
            UnitPrice = 10m,
            Quantity = 0
        };

        var lineTotal = item.LineTotal;

        Assert.Equal(0m, lineTotal);
    }

    [Fact]
    public void LineTotal_DeveRetornarZero_QuandoPrecoZero()
    {
        var item = new OrderItem
        {
            UnitPrice = 0m,
            Quantity = 10
        };

        var lineTotal = item.LineTotal;

        Assert.Equal(0m, lineTotal);
    }

    [Fact]
    public void LineTotal_DeveCalcularCorretamente_ComValoresDecimais()
    {
        var item = new OrderItem
        {
            UnitPrice = 19.99m,
            Quantity = 2
        };

        var lineTotal = item.LineTotal;

        Assert.Equal(39.98m, lineTotal);
    }
}
