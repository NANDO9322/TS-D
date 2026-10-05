using System.Text.Json;
using TSD.Models;

namespace TSD.Infrastructure;

public static class LeitorJson
{
    private static readonly JsonSerializerOptions Opcoes = new() { PropertyNameCaseInsensitive = true };
    public static DadosVendas LerVendas(string caminho) => Ler<DadosVendas>(caminho);
    public static DadosEstoque LerEstoque(string caminho) => Ler<DadosEstoque>(caminho);

    private static T Ler<T>(string caminho)
    {
        if (!File.Exists(caminho)) throw new FileNotFoundException("Arquivo JSON não encontrado.", caminho);
        using var arquivo = File.OpenRead(caminho);
        return JsonSerializer.Deserialize<T>(arquivo, Opcoes)
            ?? throw new InvalidDataException($"Não foi possível ler os dados de {caminho}.");
    }
}
