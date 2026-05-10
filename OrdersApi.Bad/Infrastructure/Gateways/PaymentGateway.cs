namespace OrdersApi.Bad.Infrastructure.Gateways;

public class PaymentGateway
{
    public bool Pay(decimal total)
    {
        return total <= 1000m;
    }
}
