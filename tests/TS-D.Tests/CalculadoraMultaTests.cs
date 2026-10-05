using TSD.Services;

namespace TSD.Tests;

public sealed class CalculadoraMultaTests
{
    private readonly CalculadoraMulta _calculadora = new();

    [Fact]
    public void Calcular_DeveAplicarDoisEMeioPorCentoPorDia()
    {
        var resultado = _calculadora.Calcular(100m, new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 5));
        Assert.Equal(4, resultado.DiasEmAtraso);
        Assert.Equal(10m, resultado.ValorMulta);
        Assert.Equal(110m, resultado.ValorTotal);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    public void Calcular_NaoDeveCobrarAntesOuNoVencimento(int diaVencimento)
    {
        var resultado = _calculadora.Calcular(100m, new DateOnly(2026, 10, diaVencimento), new DateOnly(2026, 10, 5));
        Assert.Equal(0m, resultado.ValorMulta);
    }
}
