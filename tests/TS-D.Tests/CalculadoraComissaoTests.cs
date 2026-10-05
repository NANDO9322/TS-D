using TSD.Models;
using TSD.Services;

namespace TSD.Tests;

public sealed class CalculadoraComissaoTests
{
    private readonly CalculadoraComissao _calculadora = new();

    [Theory]
    [InlineData(99.99, 0)]
    [InlineData(100, 1)]
    [InlineData(499.99, 5)]
    [InlineData(500, 25)]
    public void CalcularPorVenda_DeveRespeitarAsFaixas(decimal valor, decimal esperado) =>
        Assert.Equal(esperado, _calculadora.CalcularPorVenda(valor));

    [Fact]
    public void Calcular_DeveAgruparAsVendasPorVendedor()
    {
        var vendas = new[] { new Venda("Ana", 100m), new Venda("ana", 500m) };
        var resultado = Assert.Single(_calculadora.Calcular(vendas));
        Assert.Equal(600m, resultado.TotalVendido);
        Assert.Equal(26m, resultado.Comissao);
    }
}
