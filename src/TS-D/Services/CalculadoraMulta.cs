using TSD.Models;

namespace TSD.Services;

public sealed class CalculadoraMulta(decimal taxaDiaria = 0.025m)
{
    public decimal TaxaDiaria { get; } = taxaDiaria >= 0
        ? taxaDiaria : throw new ArgumentOutOfRangeException(nameof(taxaDiaria));

    public ResultadoMulta Calcular(decimal valor, DateOnly vencimento, DateOnly hoje)
    {
        if (valor < 0)
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor não pode ser negativo.");

        var diasEmAtraso = Math.Max(0, hoje.DayNumber - vencimento.DayNumber);
        var multa = decimal.Round(valor * TaxaDiaria * diasEmAtraso, 2, MidpointRounding.AwayFromZero);
        return new ResultadoMulta(valor, diasEmAtraso, multa, valor + multa);
    }
}
