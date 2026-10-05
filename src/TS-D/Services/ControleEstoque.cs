using TSD.Models;

namespace TSD.Services;

public sealed class ControleEstoque
{
    private readonly Dictionary<int, Produto> _produtos;
    private readonly List<MovimentacaoEstoque> _movimentacoes = [];

    public ControleEstoque(IEnumerable<Produto> produtos)
    {
        ArgumentNullException.ThrowIfNull(produtos);
        _produtos = produtos.ToDictionary(produto => produto.CodigoProduto);
        if (_produtos.Values.Any(produto => produto.Estoque < 0))
            throw new ArgumentException("O estoque inicial não pode ser negativo.", nameof(produtos));
    }

    public IReadOnlyCollection<Produto> Produtos => _produtos.Values;
    public IReadOnlyList<MovimentacaoEstoque> Movimentacoes => _movimentacoes;

    public MovimentacaoEstoque Movimentar(int codigoProduto, TipoMovimentacao tipo,
        int quantidade, string descricao)
    {
        if (!_produtos.TryGetValue(codigoProduto, out var produto))
            throw new KeyNotFoundException($"Produto {codigoProduto} não encontrado.");
        if (quantidade <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("A descrição da movimentação é obrigatória.", nameof(descricao));
        if (tipo == TipoMovimentacao.Saida && quantidade > produto.Estoque)
            throw new InvalidOperationException("A saída não pode deixar o estoque negativo.");

        produto.Estoque += tipo == TipoMovimentacao.Entrada ? quantidade : -quantidade;
        var movimentacao = new MovimentacaoEstoque(Guid.NewGuid(), codigoProduto, tipo,
            descricao.Trim(), quantidade, produto.Estoque, DateTimeOffset.Now);
        _movimentacoes.Add(movimentacao);
        return movimentacao;
    }
}
