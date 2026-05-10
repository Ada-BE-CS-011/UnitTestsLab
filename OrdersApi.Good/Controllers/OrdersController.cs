using Microsoft.AspNetCore.Mvc;
using OrdersApi.Good.Application.Interfaces;
using OrdersApi.Good.Dtos;

namespace OrdersApi.Good.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequestDto request)
    {
        var result = _orderService.CreateOrder(request);
        if (!result.Success)
        {
            return BadRequest(new { result.Message });
        }

        return Ok(result.Order);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var order = _orderService.GetOrderById(id);
        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var order = _orderService.GetAllOrders();
        return Ok(order);
    }
}
