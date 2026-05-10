using OrdersApi.Good.Domain.Entities;
using OrdersApi.Good.Infrastructure.Repositories;

namespace OrdersApi.Tests.Infrastructure.Repositories;

public class InMemoryOrderRepositoryTests
{
    [Fact]
    public void Add_DeveAtribuirId()
    {
        var repository = new InMemoryOrderRepository();
        var order = new Order();

        var result = repository.Add(order);

        Assert.True(result.Id > 0);
    }

    [Fact]
    public void Add_DeveAtribuirIdsSequenciais()
    {
        var repository = new InMemoryOrderRepository();
        var order1 = new Order();
        var order2 = new Order();

        var result1 = repository.Add(order1);
        var result2 = repository.Add(order2);

        Assert.Equal(result1.Id + 1, result2.Id);
    }

    [Fact]
    public void Add_DeveRetornarMesmoObjetoComIdAtribuido()
    {
        var repository = new InMemoryOrderRepository();
        var order = new Order();

        var result = repository.Add(order);

        Assert.Same(order, result);
    }

    [Fact]
    public void GetById_DeveRetornarNull_QuandoPedidoNaoExiste()
    {
        var repository = new InMemoryOrderRepository();

        var result = repository.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public void GetById_DeveRetornarPedido_QuandoPedidoExiste()
    {
        var repository = new InMemoryOrderRepository();
        var order = new Order();
        var added = repository.Add(order);

        var result = repository.GetById(added.Id);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetById_DeveRetornarPedidoCorreto()
    {
        var repository = new InMemoryOrderRepository();
        var order = new Order { Total = 123.45m };
        var added = repository.Add(order);

        var result = repository.GetById(added.Id);

        Assert.Equal(added.Id, result!.Id);
        Assert.Equal(123.45m, result.Total);
    }

    [Fact]
    public void GetById_DeveRetornarMesmaInstancia()
    {
        var repository = new InMemoryOrderRepository();
        var order = new Order();
        var added = repository.Add(order);

        var result = repository.GetById(added.Id);

        Assert.Same(added, result);
    }
}
