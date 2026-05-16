using Microsoft.AspNetCore.Mvc;
using OrdersApi.Bad.Domain.Entities;
using OrdersApi.Bad.Dtos;
using OrdersApi.Bad.Infrastructure.Database;
using OrdersApi.Bad.Infrastructure.Gateways;
using OrdersApi.Bad.Infrastructure.Repositories;

namespace OrdersApi.Bad.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public OrdersController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpPost]
    public IActionResult Create(CreateOrderRequest request)
    {
        var productRepository = new ProductRepository(dbContext);
        var orderRepository = new OrderRepository(dbContext);
        var paymentGateway = new PaymentGateway();

        // OK - Se os dados a order for nula, retorna BadRequest
        if (request == null)
        {
            return BadRequest("request is null");
        }

        // OK - Se a order não tiver itens, retorna badrequest
        if (request.Items == null || request.Items.Count == 0)
        {
            return BadRequest("pedido precisa de item");
        }

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
            {
                return BadRequest("qty invalida");
            }

            // Produto não Existe
            var product = productRepository.GetById(incoming.ProductId);
            if (product == null)
            {
                return BadRequest("produto nao existe");
            }

            if (incoming.Quantity > product.Stock)
            {
                return BadRequest("sem estoque");
            }

            var item = new OrderItem();
            item.ProductId = incoming.ProductId;
            item.Quantity = incoming.Quantity;
            item.UnitPrice = product.Price;

            subtotal = subtotal + (product.Price * incoming.Quantity);
            order.Items.Add(item);
        }

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

        decimal shipping = 20m;
        if (subtotal > 300m)
        {
            shipping = 0m;
        }

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

        return Ok(created);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var orderRepository = new OrderRepository(dbContext);
        var order = orderRepository.GetById(id);
        if (order == null)
        {
            return NotFound("order not found");
        }

        return Ok(order);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var orderRepository = new OrderRepository(dbContext);
        var orders = orderRepository.GetAll();
        return Ok(orders);
    }
}
