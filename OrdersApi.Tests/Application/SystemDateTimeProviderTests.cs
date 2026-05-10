using OrdersApi.Good.Application;

namespace OrdersApi.Tests.Application;

public class SystemDateTimeProviderTests
{
    [Fact]
    public void UtcNow_DeveRetornarDataAtual()
    {
        var provider = new SystemDateTimeProvider();
        var before = DateTime.UtcNow;

        var result = provider.UtcNow;

        var after = DateTime.UtcNow;
        Assert.InRange(result, before, after);
    }

    [Fact]
    public void UtcNow_DeveRetornarDateTimeEmUtc()
    {
        var provider = new SystemDateTimeProvider();

        var result = provider.UtcNow;

        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }
}
