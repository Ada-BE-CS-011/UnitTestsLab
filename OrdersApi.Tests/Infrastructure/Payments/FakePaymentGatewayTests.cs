using OrdersApi.Good.Infrastructure.Payments;

namespace OrdersApi.Tests.Infrastructure.Payments;

public class FakePaymentGatewayTests
{
    [Fact]
    public void Process_DeveAprovar_QuandoValorMenorQue1000()
    {
        var gateway = new FakePaymentGateway();

        var result = gateway.Process(999.99m);

        Assert.True(result);
    }

    [Fact]
    public void Process_DeveAprovar_QuandoValorExatamente1000()
    {
        var gateway = new FakePaymentGateway();

        var result = gateway.Process(1000m);

        Assert.True(result);
    }

    [Fact]
    public void Process_DeveRecusar_QuandoValorMaiorQue1000()
    {
        var gateway = new FakePaymentGateway();

        var result = gateway.Process(1000.01m);

        Assert.False(result);
    }

    [Fact]
    public void Process_DeveAprovar_QuandoValorZero()
    {
        var gateway = new FakePaymentGateway();

        var result = gateway.Process(0m);

        Assert.True(result);
    }

    [Fact]
    public void Process_DeveAprovar_QuandoValorNegativo()
    {
        var gateway = new FakePaymentGateway();

        var result = gateway.Process(-10m);

        Assert.True(result);
    }
}
