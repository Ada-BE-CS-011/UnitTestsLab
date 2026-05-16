using Microsoft.AspNetCore.Mvc;
using OrdersApi.Bad.Application.Services;
using OrdersApi.Bad.Infrastructure.Database;
using OrdersApi.Bad.Infrastructure.Repositories;

namespace OrdersApi.Bad.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext dbContext;
    private readonly IOrderService service;

    public ProductsController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var repo = new ProductRepository(dbContext);
        return Ok(repo.GetAll());
    }
}
