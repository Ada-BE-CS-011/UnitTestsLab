using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Bad.Domain.Entities;
using OrdersApi.Bad.Dtos;
using OrdersApi.Bad.Infrastructure.Repositories;
using OrdersApi.Bad.Infrastructure.Gateways;

namespace OrdersApi.Bad.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpPost]
    public IActionResult Create(CreateOrderRequest request)
    {
        var productRepository = new ProductRepository();
        var orderRepository = new OrderRepository();
        var paymentGateway = new PaymentGateway();

        if (request == null)
        {
            return BadRequest("request is null");
        }

        if (request.Items == null || request.Items.Count == 0)
        {
            return BadRequest("pedido precisa de item");
        }

        var order = new Order();
        order.Status = "Pending";
        order.CreatedAt = DateTime.Now;
        order.Coupon = request.Coupon ?? "";

        decimal subtotal = 0m;

        for (int i = 0; i < request.Items.Count; i++)
        {
            var incoming = request.Items[i];

            if (incoming.Quantity <= 0)
            {
                return BadRequest("qty invalida");
            }

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
        var orderRepository = new OrderRepository();
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
        var orderRepository = new OrderRepository();
        var orders = orderRepository.GetAll();
        return Ok(orders);
    }
}
