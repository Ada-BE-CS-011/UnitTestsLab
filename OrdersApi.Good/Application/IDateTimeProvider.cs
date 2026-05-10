namespace OrdersApi.Good.Application;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
