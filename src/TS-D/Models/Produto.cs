namespace TSD.Models;
public sealed class Produto
{
    public int CodigoProduto { get; init; }
    public string DescricaoProduto { get; init; } = string.Empty;
    public int Estoque { get; set; }
}
