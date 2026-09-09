using ControleDeEstoque.WebApp.Compartilhado.Arquivos;
using ControleDeEstoque.WebApp.ModuloFuncionario;
using ControleDeEstoque.WebApp.ModuloProdutos;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public class RequisicaoEntradaController : Controller
{
    private readonly RepositorioRequisicaoEntradaEmArquivo repositorio;
    private readonly RepositorioProdutoEmArquivo repositorioProduto;
    private readonly RepositorioFuncionarioEmArquivo repositorioFuncionario;

    public RequisicaoEntradaController(
        RepositorioRequisicaoEntradaEmArquivo repositorio,
        RepositorioProdutoEmArquivo repositorioProduto,
        RepositorioFuncionarioEmArquivo repositorioFuncionario)
    {
        this.repositorio = repositorio;
        this.repositorioProduto = repositorioProduto;
        this.repositorioFuncionario = repositorioFuncionario;
    }

    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarRequisicaoEntradaViewModel> viewModels = [];

        foreach (RequisicaoEntrada requisicao in repositorio.SelecionarTodos())
        {
            ListarRequisicaoEntradaViewModel viewModel = new ListarRequisicaoEntradaViewModel(
                requisicao.Id,
                requisicao.Produto.Nome,
                requisicao.Funcionario.Nome,
                requisicao.Quantidade,
                requisicao.Data
            );

            viewModels.Add(viewModel);
        }

        return View(viewModels);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarRequisicaoEntradaViewModel viewModel = new CadastrarRequisicaoEntradaViewModel(
            0,
            0,
            0
        ) with
        { Produtos = ObterProdutos(), Funcionarios = ObterFuncionarios() };

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarRequisicaoEntradaViewModel viewModel)
    {
        Produto? produto = repositorioProduto.SelecionarPorId(viewModel.ProdutoId);

        if (produto == null)
            return NotFound();

        Funcionario? funcionario = repositorioFuncionario.SelecionarPorId(viewModel.FuncionarioId);

        if (funcionario == null)
            return NotFound();

        RequisicaoEntrada requisicaoEntrada = new RequisicaoEntrada(
            produto,
            viewModel.Quantidade,
            funcionario
        );

        repositorio.Cadastrar(requisicaoEntrada);

        return RedirectToAction(nameof(Listar));
    }

    private List<ProdutoRequisicaoEntradaViewModel> ObterProdutos()
    {
        List<ProdutoRequisicaoEntradaViewModel> viewModels = [];

        foreach (Produto produto in repositorioProduto.SelecionarTodos())
        {
            ProdutoRequisicaoEntradaViewModel viewModel = new ProdutoRequisicaoEntradaViewModel(
                produto.Id,
                produto.Nome
            );

            viewModels.Add(viewModel);
        }

        return viewModels;
    }

    private List<FuncionarioRequisicaoEntradaViewModel> ObterFuncionarios()
    {
        List<FuncionarioRequisicaoEntradaViewModel> viewModels = [];

        foreach (Funcionario funcionario in repositorioFuncionario.SelecionarTodos())
        {
            FuncionarioRequisicaoEntradaViewModel viewModel = new FuncionarioRequisicaoEntradaViewModel(
                funcionario.Id,
                funcionario.Nome
            );

            viewModels.Add(viewModel);
        }

        return viewModels;
    }
}
