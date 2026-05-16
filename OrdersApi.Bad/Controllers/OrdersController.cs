using Microsoft.AspNetCore.Mvc;
using OrdersApi.Bad.Application.Services;
using OrdersApi.Bad.Dtos;
using OrdersApi.Bad.Infrastructure.Database;
using OrdersApi.Bad.Infrastructure.Extensions;
using OrdersApi.Bad.Infrastructure.Repositories;

namespace OrdersApi.Bad.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext dbContext;
    private readonly IOrderService service;

    public OrdersController(AppDbContext dbContext, IOrderService service)
    {
        this.dbContext = dbContext;
        this.service = service;
    }

    [HttpPost]
    public IActionResult Create(CreateOrderRequest request)
    {
        var result = service.CreateOrder(request);
        return result.ToActionResult();
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
