using System.Globalization;
using System.Text;
using TSD.Infrastructure;
using TSD.Models;
using TSD.Services;

namespace TSD;

public static class Program
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");

    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        try
        {
            var pastaDados = Path.Combine(AppContext.BaseDirectory, "Data");
            var vendas = LeitorJson.LerVendas(Path.Combine(pastaDados, "vendas.json"));
            var estoque = LeitorJson.LerEstoque(Path.Combine(pastaDados, "estoque.json"));
            ExibirMenu(vendas, new ControleEstoque(estoque.Estoque));
        }
        catch (Exception excecao)
        {
            Console.Error.WriteLine($"Não foi possível iniciar o programa: {excecao.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static void ExibirMenu(DadosVendas dadosVendas, ControleEstoque controleEstoque)
    {
        while (true)
        {
            Console.WriteLine("\n=== TS-D ===");
            Console.WriteLine("1 - Calcular comissões\n2 - Movimentar estoque\n3 - Calcular multa por atraso\n0 - Sair");
            Console.Write("Escolha uma opção: ");
            switch (Console.ReadLine())
            {
                case "1": ExibirComissoes(dadosVendas); break;
                case "2": MovimentarEstoque(controleEstoque); break;
                case "3": CalcularMulta(); break;
                case "0": return;
                default: Console.WriteLine("Opção inválida. Tente novamente."); break;
            }
        }
    }

    private static void ExibirComissoes(DadosVendas dados)
    {
        Console.WriteLine("\nComissões por vendedor");
        foreach (var resultado in new CalculadoraComissao().Calcular(dados.Vendas))
            Console.WriteLine($"{resultado.Vendedor,-20} Vendas: {resultado.TotalVendido.ToString("C", Cultura),12} | Comissão: {resultado.Comissao.ToString("C", Cultura),10}");
    }

    private static void MovimentarEstoque(ControleEstoque controle)
    {
        Console.WriteLine("\nProdutos");
        foreach (var produto in controle.Produtos.OrderBy(produto => produto.CodigoProduto))
            Console.WriteLine($"{produto.CodigoProduto} - {produto.DescricaoProduto} ({produto.Estoque} unidades)");

        var codigo = LerInteiro("Código do produto: ");
        var tipo = LerTipoMovimentacao();
        var quantidade = LerInteiro("Quantidade: ");
        Console.Write("Descrição da movimentação: ");
        var descricao = Console.ReadLine() ?? string.Empty;

        try
        {
            var movimento = controle.Movimentar(codigo, tipo, quantidade, descricao);
            Console.WriteLine($"Movimentação {movimento.Id} registrada. Estoque final: {movimento.EstoqueFinal}.");
        }
        catch (Exception excecao) when (excecao is ArgumentException or InvalidOperationException or KeyNotFoundException)
        {
            Console.WriteLine($"Movimentação não realizada: {excecao.Message}");
        }
    }

    private static void CalcularMulta()
    {
        var valor = LerDecimal("Valor original: R$ ");
        var vencimento = LerData("Data de vencimento (dd/MM/aaaa): ");
        var resultado = new CalculadoraMulta().Calcular(valor, vencimento, DateOnly.FromDateTime(DateTime.Today));
        Console.WriteLine($"Dias em atraso: {resultado.DiasEmAtraso}");
        Console.WriteLine($"Multa: {resultado.ValorMulta.ToString("C", Cultura)}");
        Console.WriteLine($"Total atualizado: {resultado.ValorTotal.ToString("C", Cultura)}");
    }

    private static int LerInteiro(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            if (int.TryParse(Console.ReadLine(), out var valor)) return valor;
            Console.WriteLine("Digite um número inteiro válido.");
        }
    }

    private static decimal LerDecimal(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, Cultura, out var valor)) return valor;
            Console.WriteLine("Digite um valor válido, por exemplo 150,50.");
        }
    }

    private static DateOnly LerData(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            if (DateOnly.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", Cultura, DateTimeStyles.None, out var data)) return data;
            Console.WriteLine("Digite a data no formato dd/MM/aaaa.");
        }
    }

    private static TipoMovimentacao LerTipoMovimentacao()
    {
        while (true)
        {
            Console.Write("Tipo (E = entrada / S = saída): ");
            switch (Console.ReadLine()?.Trim().ToUpperInvariant())
            {
                case "E": return TipoMovimentacao.Entrada;
                case "S": return TipoMovimentacao.Saida;
                default: Console.WriteLine("Informe E para entrada ou S para saída."); break;
            }
        }
    }
}
