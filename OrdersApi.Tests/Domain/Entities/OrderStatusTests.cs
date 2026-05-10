using OrdersApi.Good.Domain.Entities;

namespace OrdersApi.Tests.Domain.Entities;

public class OrderStatusTests
{
    [Fact]
    public void Pending_DeveSerValor0()
    {
        var status = OrderStatus.Pending;

        Assert.Equal(0, (int)status);
    }

    [Fact]
    public void Approved_DeveSerValor1()
    {
        var status = OrderStatus.Approved;

        Assert.Equal(1, (int)status);
    }

    [Fact]
    public void Rejected_DeveSerValor2()
    {
        var status = OrderStatus.Rejected;

        Assert.Equal(2, (int)status);
    }

    [Fact]
    public void ToString_DeveRetornarNomeDoStatus()
    {
        var status = OrderStatus.Approved;

        var result = status.ToString();

        Assert.Equal("Approved", result);
    }
}
