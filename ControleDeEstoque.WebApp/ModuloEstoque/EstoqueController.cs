using ControleDeEstoque.WebApp.Compartilhado.Arquivos;
using ControleDeEstoque.WebApp.ModuloProdutos;
using ControleDeEstoque.WebApp.ModuloRequisicoes;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeEstoque.WebApp.ModuloEstoque;

public class EstoqueController : Controller
{
    private readonly RepositorioProdutoEmArquivo repositorioProduto;
    private readonly RepositorioRequisicaoEntradaEmArquivo repositorioEntrada;
    private readonly RepositorioRequisicaoSaidaEmArquivo repositorioSaida;

    public EstoqueController(
        RepositorioProdutoEmArquivo repositorioProduto,
        RepositorioRequisicaoEntradaEmArquivo repositorioEntrada,
        RepositorioRequisicaoSaidaEmArquivo repositorioSaida)
    {
        this.repositorioProduto = repositorioProduto;
        this.repositorioEntrada = repositorioEntrada;
        this.repositorioSaida = repositorioSaida;
    }

    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarEstoqueViewModel> viewModels = [];

        foreach (Produto produto in repositorioProduto.SelecionarTodos())
        {
            viewModels.Add(new ListarEstoqueViewModel(
                produto.Id,
                produto.Nome,
                produto.Categoria.ToString(),
                produto.QuantidadeEmEstoque
            ));
        }

        return View(viewModels);
    }

    [HttpGet]
    public ActionResult RelatorioSemanal()
    {
        DateTime fim = DateTime.Now;
        DateTime inicio = fim.Date.AddDays(-6);
        DateTime fimExclusivo = fim.Date.AddDays(1);

        List<RelatorioEntradaViewModel> entradas = [];
        foreach (RequisicaoEntrada entrada in repositorioEntrada.SelecionarTodos())
        {
            if (entrada.Data < inicio || entrada.Data >= fimExclusivo)
                continue;

            string documento = entrada.Tipo == TipoEntrada.NotaFiscal
                ? entrada.NumeroNotaFiscal ?? "-"
                : "-";

            entradas.Add(new RelatorioEntradaViewModel(
                entrada.Data,
                entrada.Produto.Nome,
                entrada.Quantidade,
                entrada.Tipo == TipoEntrada.NotaFiscal ? "Nota Fiscal" : "Devolução",
                documento,
                entrada.Funcionario.Nome
            ));
        }

        List<RelatorioSaidaViewModel> saídas = [];
        foreach (RequisicaoSaida saida in repositorioSaida.SelecionarTodos())
        {
            if (saida.Data < inicio || saida.Data >= fimExclusivo)
                continue;

            foreach (ProdutoPrescrito prescrito in saida.ProdutosPrescritos)
            {
                saídas.Add(new RelatorioSaidaViewModel(
                    saida.Data,
                    saida.Cliente.Nome,
                    prescrito.Produto.Nome,
                    prescrito.Quantidade
                ));
            }
        }

        entradas = entradas.OrderByDescending(x => x.Data).ToList();
        saídas = saídas.OrderByDescending(x => x.Data).ToList();

        RelatorioSemanalViewModel viewModel = new(
            inicio,
            fim.Date,
            entradas,
            saídas
        );

        return View(viewModel);
    }
}
