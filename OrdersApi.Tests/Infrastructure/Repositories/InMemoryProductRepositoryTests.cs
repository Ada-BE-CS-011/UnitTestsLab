using OrdersApi.Good.Infrastructure.Repositories;

namespace OrdersApi.Tests.Infrastructure.Repositories;

public class InMemoryProductRepositoryTests
{
    [Fact]
    public void GetAll_DeveRetornar4Produtos()
    {
        var repository = new InMemoryProductRepository();

        var products = repository.GetAll();

        Assert.Equal(4, products.Count);
    }

    [Fact]
    public void GetAll_DeveRetornarProdutosNaOrdemCorreta()
    {
        var repository = new InMemoryProductRepository();

        var products = repository.GetAll();

        Assert.Equal("Notebook", products[0].Name);
        Assert.Equal("Mouse", products[1].Name);
        Assert.Equal("Teclado", products[2].Name);
        Assert.Equal("Monitor", products[3].Name);
    }

    [Fact]
    public void GetById_DeveRetornarNull_QuandoProdutoNaoExiste()
    {
        var repository = new InMemoryProductRepository();

        var product = repository.GetById(999);

        Assert.Null(product);
    }

    [Fact]
    public void GetById_DeveRetornarProduto_QuandoProdutoExiste()
    {
        var repository = new InMemoryProductRepository();

        var product = repository.GetById(1);

        Assert.NotNull(product);
    }

    [Fact]
    public void GetById_DeveRetornarProdutoCorreto()
    {
        var repository = new InMemoryProductRepository();

        var product = repository.GetById(2);

        Assert.Equal(2, product!.Id);
        Assert.Equal("Mouse", product.Name);
        Assert.Equal(80m, product.Price);
        Assert.Equal(20, product.Stock);
    }

    [Fact]
    public void Update_DeveAtualizarProduto()
    {
        var repository = new InMemoryProductRepository();
        var product = repository.GetById(2)!;
        product.Stock = 15;

        repository.Update(product);

        var updatedProduct = repository.GetById(2);
        Assert.Equal(15, updatedProduct!.Stock);
    }

    [Fact]
    public void Update_NaoDeveAlterarOutrosProdutos()
    {
        var repository = new InMemoryProductRepository();
        var product = repository.GetById(2)!;
        var originalStock3 = repository.GetById(3)!.Stock;
        product.Stock = 15;

        repository.Update(product);

        var product3 = repository.GetById(3);
        Assert.Equal(originalStock3, product3!.Stock);
    }

    [Fact]
    public void Update_NaoDeveFazerNada_QuandoProdutoNaoExiste()
    {
        var repository = new InMemoryProductRepository();
        var product = new Good.Domain.Entities.Product { Id = 999, Name = "Fake", Price = 1, Stock = 1 };

        repository.Update(product);

        var notFound = repository.GetById(999);
        Assert.Null(notFound);
    }
}
