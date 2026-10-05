namespace TSD.Models;
public enum TipoMovimentacao { Entrada, Saida }
public sealed record MovimentacaoEstoque(Guid Id, int CodigoProduto, TipoMovimentacao Tipo,
    string Descricao, int Quantidade, int EstoqueFinal, DateTimeOffset RealizadaEm);
