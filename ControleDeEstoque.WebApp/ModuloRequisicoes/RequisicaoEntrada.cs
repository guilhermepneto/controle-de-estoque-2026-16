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
            ListarRequisicaoEntradaViewModel viewModel = new(
                requisicao.Id,
                requisicao.Produto.Nome,
                requisicao.Funcionario.Nome,
                requisicao.Quantidade,
                requisicao.Data,
                requisicao.Tipo,
                requisicao.NumeroNotaFiscal
            );

            viewModels.Add(viewModel);
        }

        return View(viewModels);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarRequisicaoEntradaViewModel viewModel = new(0, 0, 0)
        {
            Produtos = ObterProdutos(),
            Funcionarios = ObterFuncionarios()
        };

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

        string? numeroNotaFiscal = viewModel.Tipo == TipoEntrada.NotaFiscal
            ? viewModel.NumeroNotaFiscal
            : null;

        RequisicaoEntrada requisicaoEntrada = new(
            produto,
            viewModel.Quantidade,
            funcionario,
            viewModel.Tipo,
            numeroNotaFiscal
        );

        List<string> erros = requisicaoEntrada.Validar();

        if (erros.Count > 0)
        {
            foreach (string erro in erros)
                ModelState.AddModelError(string.Empty, erro);

            viewModel = viewModel with
            {
                Produtos = ObterProdutos(),
                Funcionarios = ObterFuncionarios()
            };

            return View(viewModel);
        }

        repositorio.Cadastrar(requisicaoEntrada);

        return RedirectToAction(nameof(Listar));
    }

    private List<ProdutoRequisicaoEntradaViewModel> ObterProdutos()
    {
        List<ProdutoRequisicaoEntradaViewModel> viewModels = [];

        foreach (Produto produto in repositorioProduto.SelecionarTodos())
        {
            viewModels.Add(new ProdutoRequisicaoEntradaViewModel(produto.Id, produto.Nome));
        }

        return viewModels;
    }

    private List<FuncionarioRequisicaoEntradaViewModel> ObterFuncionarios()
    {
        List<FuncionarioRequisicaoEntradaViewModel> viewModels = [];

        foreach (Funcionario funcionario in repositorioFuncionario.SelecionarTodos())
        {
            viewModels.Add(new FuncionarioRequisicaoEntradaViewModel(funcionario.Id, funcionario.Nome));
        }

        return viewModels;
    }
}
