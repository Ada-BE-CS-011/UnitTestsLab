using OrdersApi.Bad.Domain.Interfaces;

namespace OrdersApi.Bad.Infrastructure.Gateways;

public class PaymentGateway : IPaymentGateway
{
    public bool Pay(decimal total)
    {
        return total <= 1000m;
    }
}
