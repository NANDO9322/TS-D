using TSD.Models;

namespace TSD.Services;

public sealed class CalculadoraComissao
{
    public IReadOnlyList<ResultadoComissao> Calcular(IEnumerable<Venda> vendas)
    {
        ArgumentNullException.ThrowIfNull(vendas);
        var lista = vendas.ToList();

        if (lista.Any(venda => string.IsNullOrWhiteSpace(venda.Vendedor)))
            throw new ArgumentException("Toda venda deve possuir um vendedor.", nameof(vendas));
        if (lista.Any(venda => venda.Valor < 0))
            throw new ArgumentException("O valor de uma venda não pode ser negativo.", nameof(vendas));

        return lista
            .GroupBy(venda => venda.Vendedor.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(grupo => new ResultadoComissao(
                grupo.First().Vendedor.Trim(), grupo.Sum(venda => venda.Valor),
                grupo.Sum(venda => CalcularPorVenda(venda.Valor))))
            .OrderBy(resultado => resultado.Vendedor)
            .ToList();
    }

    public decimal CalcularPorVenda(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor da venda não pode ser negativo.");

        var percentual = valor switch { < 100m => 0m, < 500m => 0.01m, _ => 0.05m };
        return decimal.Round(valor * percentual, 2, MidpointRounding.AwayFromZero);
    }
}
