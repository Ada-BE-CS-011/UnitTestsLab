using OrdersApi.Good.Domain.Interfaces;

namespace OrdersApi.Good.Infrastructure.Payments;

public class FakePaymentGateway : IPaymentGateway
{
    public bool Process(decimal amount)
    {
        // Regra fake para didática: acima de 1000 é recusado.
        return amount <= 1000m;
    }
}
