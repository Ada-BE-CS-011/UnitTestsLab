using Microsoft.AspNetCore.Mvc;
using OrdersApi.Bad.Infrastructure.Repositories;

namespace OrdersApi.Bad.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var repo = new ProductRepository();
        return Ok(repo.GetAll());
    }
}
