namespace OrdersApi.Bad.Domain.Interfaces
{
    public interface IPaymentGateway
    {
        bool Pay(decimal total);
    }
}