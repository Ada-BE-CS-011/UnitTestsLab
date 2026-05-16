using ErrorOr;
using OrdersApi.Bad.Domain.Entities;
using OrdersApi.Bad.Domain.Interfaces;
using OrdersApi.Bad.Dtos;

namespace OrdersApi.Bad.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IPaymentGateway paymentGateway;

        public OrderService(IProductRepository productRepository,
                            IOrderRepository orderRepository,
                            IPaymentGateway paymentGateway
          )
        {
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.paymentGateway = paymentGateway;
        }

        public ErrorOr<Order?> CreateOrder(CreateOrderRequest request)
        {
            // OK - Se os dados a order for nula, retorna BadRequest
            if (request == null)
                return Error.Validation(nameof(request), "Request is Null");

            // OK - Se a order não tiver itens, retorna badrequest
            if (request.Items == null || request.Items.Count == 0)
                return Error.Validation(nameof(request.Items), "The order must contain at least one item");

            // Status Inicial da Order é Pending
            var order = new Order();
            order.Status = "Pending";
            order.CreatedAt = DateTime.Now;
            order.Coupon = request.Coupon ?? "";

            decimal subtotal = 0m;

            for (int i = 0; i < request.Items.Count; i++)
            {
                var incoming = request.Items[i];

                // Quantidade de Items é Inválida
                if (incoming.Quantity <= 0)
                    return Error.Validation(nameof(request.Items), "The Item quantity must be greater then zero");

                // Produto não Existe
                var product = productRepository.GetById(incoming.ProductId);
                if (product == null)
                    return Error.NotFound(nameof(Product), $"The product with id {incoming.ProductId} was not found");

                // Produto tem Estoque
                if (incoming.Quantity > product.Stock)
                    return Error.Unexpected(nameof(Product), $"The Product {product.Name} is out of stock");

                var item = new OrderItem();
                item.ProductId = incoming.ProductId;
                item.Quantity = incoming.Quantity;
                item.UnitPrice = product.Price;

                // Calcula o subtotal corretamente?
                subtotal = subtotal + (product.Price * incoming.Quantity);
                order.Items.Add(item);
            }

            // Calcula o Desconto Corretamente?
            decimal discount = 0m;
            if (order.Coupon == "DESC10")
            {
                discount = subtotal * 0.10m;
            }
            else if (order.Coupon == "VIP20")
            {
                if (subtotal > 500m)
                {
                    discount = subtotal * 0.20m;
                }
            }

            // Calcula o Valor do Frete Corretamente?
            decimal shipping = 20m;
            if (subtotal > 300m)
            {
                shipping = 0m;
            }

            // Calcula o Total Corretamente?
            var total = subtotal - discount + shipping;

            var paymentOk = paymentGateway.Pay(total);
            if (paymentOk)
            {
                order.Status = "Approved";

                for (int i = 0; i < order.Items.Count; i++)
                {
                    var item = order.Items[i];
                    var p = productRepository.GetById(item.ProductId);
                    if (p != null)
                    {
                        p.Stock = p.Stock - item.Quantity;
                    }
                }
            }
            else
            {
                order.Status = "Rejected";
            }

            order.Subtotal = subtotal;
            order.Discount = discount;
            order.Shipping = shipping;
            order.Total = total;

            var created = orderRepository.Add(order);

            return created;
        }
    }
}
