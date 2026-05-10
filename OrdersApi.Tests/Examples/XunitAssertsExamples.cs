using OrdersApi.Good.Dtos;

namespace OrdersApi.Tests.Examples;

/// <summary>
/// Classe didática demonstrando os principais asserts do xUnit.
/// Cada teste foca em um tipo específico de assert.
/// </summary>
public class XunitAssertsExamples
{
    // ========== COMPARAÇÃO ==========

    [Fact]
    public void Assert_Equal_DeveCompararValoresIguais()
    {
        var expectedPrice = 100m;
        var actualPrice = 100m;

        Assert.Equal(expectedPrice, actualPrice);
    }

    [Fact]
    public void Assert_NotEqual_DeveCompararValoresDiferentes()
    {
        var price1 = 100m;
        var price2 = 200m;

        Assert.NotEqual(price1, price2);
    }

    [Fact]
    public void Assert_InRange_DeveVerificarValorDentroDoIntervalo()
    {
        var orderTotal = 150m;

        Assert.InRange(orderTotal, 100m, 200m);
    }

    [Fact]
    public void Assert_Equivalent_DeveCompararObjetosComMesmosValores()
    {
        var expected = new { Id = 1, Name = "Mouse" };
        var actual = new { Id = 1, Name = "Mouse" };

        Assert.Equivalent(expected, actual);
    }

    // ========== BOOLEANOS ==========

    [Fact]
    public void Assert_True_DeveVerificarCondicaoVerdadeira()
    {
        var isApproved = true;

        Assert.True(isApproved);
    }

    [Fact]
    public void Assert_False_DeveVerificarCondicaoFalsa()
    {
        var isRejected = false;

        Assert.False(isRejected);
    }

    // ========== NULIDADE ==========

    [Fact]
    public void Assert_Null_DeveVerificarValorNulo()
    {
        string? coupon = null;

        Assert.Null(coupon);
    }

    [Fact]
    public void Assert_NotNull_DeveVerificarValorNaoNulo()
    {
        var order = new OrderResponseDto();

        Assert.NotNull(order);
    }

    // ========== COLEÇÕES ==========

    [Fact]
    public void Assert_Empty_DeveVerificarColecaoVazia()
    {
        var emptyList = new List<int>();

        Assert.Empty(emptyList);
    }

    [Fact]
    public void Assert_NotEmpty_DeveVerificarColecaoComElementos()
    {
        var items = new List<string> { "Mouse", "Teclado" };

        Assert.NotEmpty(items);
    }

    [Fact]
    public void Assert_Contains_DeveVerificarElementoNaColecao()
    {
        var products = new List<string> { "Mouse", "Teclado", "Monitor" };

        Assert.Contains("Mouse", products);
    }

    [Fact]
    public void Assert_DoesNotContain_DeveVerificarElementoAusenteNaColecao()
    {
        var products = new List<string> { "Mouse", "Teclado" };

        Assert.DoesNotContain("Notebook", products);
    }

    [Fact]
    public void Assert_Single_DeveVerificarColecaoComUmElemento()
    {
        var items = new List<string> { "Mouse" };

        Assert.Single(items);
    }

    [Fact]
    public void Assert_Single_WithPredicate_DeveEncontrarUnicoElemento()
    {
        var items = new List<int> { 1, 2, 3, 4, 5 };

        var result = Assert.Single(items, x => x == 3);

        Assert.Equal(3, result);
    }

    // ========== TIPOS ==========

    [Fact]
    public void Assert_IsType_DeveVerificarTipoExato()
    {
        var dto = new OrderResponseDto();

        Assert.IsType<OrderResponseDto>(dto);
    }

    [Fact]
    public void Assert_IsAssignableFrom_DeveVerificarTipoCompativel()
    {
        var items = new List<OrderItemResponseDto>();

        Assert.IsAssignableFrom<IEnumerable<OrderItemResponseDto>>(items);
    }

    // ========== STRINGS ==========

    [Fact]
    public void Assert_StartsWith_DeveVerificarInicioString()
    {
        var orderCode = "ORD-2026-001";

        Assert.StartsWith("ORD-", orderCode);
    }

    [Fact]
    public void Assert_EndsWith_DeveVerificarFimString()
    {
        var orderCode = "ORD-2026-001";

        Assert.EndsWith("-001", orderCode);
    }

    [Fact]
    public void Assert_Matches_DeveVerificarPadraoRegex()
    {
        var orderCode = "ORD-2026-001";

        Assert.Matches(@"^ORD-\d{4}-\d{3}$", orderCode);
    }

    // ========== EXCEÇÕES ==========

    [Fact]
    public void Assert_Throws_DeveCapturarExcecaoEsperada()
    {
        Action action = () => int.Parse("invalid");

        var exception = Assert.Throws<FormatException>(action);

        Assert.NotNull(exception);
    }

    [Fact]
    public void Assert_Throws_Generic_DeveCapturarExcecaoComMensagem()
    {
        Action action = () => throw new InvalidOperationException("Operation not allowed");

        var exception = Assert.Throws<InvalidOperationException>(action);

        Assert.Contains("not allowed", exception.Message);
    }

    // ========== REFERÊNCIAS ==========

    [Fact]
    public void Assert_Same_DeveVerificarMesmaReferencia()
    {
        var dto = new ProductResponseDto { Id = 1 };
        var sameReference = dto;

        Assert.Same(dto, sameReference);
    }

    [Fact]
    public void Assert_NotSame_DeveVerificarReferenciasDiferentes()
    {
        var dto1 = new ProductResponseDto { Id = 1 };
        var dto2 = new ProductResponseDto { Id = 1 };

        Assert.NotSame(dto1, dto2);
    }

    // ========== OUTROS ASSERTS ÚTEIS ==========

    [Fact]
    public void Assert_All_DeveVerificarCondicaoParaTodosElementos()
    {
        var prices = new List<decimal> { 10m, 20m, 30m };

        Assert.All(prices, price => Assert.True(price > 0));
    }

    [Fact]
    public void Assert_Collection_DeveVerificarCadaElementoDaColecao()
    {
        var products = new List<string> { "Mouse", "Teclado", "Monitor" };

        Assert.Collection(products,
            item => Assert.Equal("Mouse", item),
            item => Assert.Equal("Teclado", item),
            item => Assert.Equal("Monitor", item));
    }
}
