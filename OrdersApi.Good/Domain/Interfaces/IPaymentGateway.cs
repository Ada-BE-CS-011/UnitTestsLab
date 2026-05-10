namespace OrdersApi.Good.Domain.Interfaces;

public interface IPaymentGateway
{
    bool Process(decimal amount);
}
