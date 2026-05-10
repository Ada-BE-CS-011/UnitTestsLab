using Microsoft.AspNetCore.Mvc;
using OrdersApi.Good.Domain.Interfaces;
using OrdersApi.Good.Dtos;

namespace OrdersApi.Good.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProductResponseDto>> Get()
    {
        var products = _productRepository
            .GetAll()
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            })
            .ToList();

        return Ok(products);
    }
}
