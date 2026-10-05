using TSD.Models;
using TSD.Services;

namespace TSD.Tests;

public sealed class ControleEstoqueTests
{
    private static ControleEstoque CriarControle() =>
        new([new Produto { CodigoProduto = 101, DescricaoProduto = "Caneta", Estoque = 10 }]);

    [Fact]
    public void Entrada_DeveAumentarOEstoqueEGerarIdentificador()
    {
        var movimento = CriarControle().Movimentar(101, TipoMovimentacao.Entrada, 5, "Compra");
        Assert.Equal(15, movimento.EstoqueFinal);
        Assert.NotEqual(Guid.Empty, movimento.Id);
    }

    [Fact]
    public void Saida_DeveDiminuirOEstoque()
    {
        var movimento = CriarControle().Movimentar(101, TipoMovimentacao.Saida, 4, "Venda");
        Assert.Equal(6, movimento.EstoqueFinal);
    }

    [Fact]
    public void Saida_NaoDevePermitirEstoqueNegativo()
    {
        var controle = CriarControle();
        Assert.Throws<InvalidOperationException>(() =>
            controle.Movimentar(101, TipoMovimentacao.Saida, 11, "Venda"));
        Assert.Equal(10, controle.Produtos.Single().Estoque);
    }
}
