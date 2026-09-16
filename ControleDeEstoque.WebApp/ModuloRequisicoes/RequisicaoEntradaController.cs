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
            viewModels.Add(new ListarRequisicaoEntradaViewModel(
                requisicao.Id,
                requisicao.Produto.Nome,
                requisicao.Funcionario.Nome,
                requisicao.Quantidade,
                requisicao.Data,
                requisicao.Tipo,
                requisicao.NumeroNotaFiscal));
        }

        return View(viewModels);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarRequisicaoEntradaViewModel viewModel = new CadastrarRequisicaoEntradaViewModel(0, 0, 0)
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
        Funcionario? funcionario = repositorioFuncionario.SelecionarPorId(viewModel.FuncionarioId);

        if (produto == null)
            ModelState.AddModelError(nameof(viewModel.ProdutoId), "Selecione um produto.");

        if (funcionario == null)
            ModelState.AddModelError(nameof(viewModel.FuncionarioId), "Selecione um funcionário.");

        if (produto == null || funcionario == null)
        {
            viewModel = viewModel with
            {
                Produtos = ObterProdutos(),
                Funcionarios = ObterFuncionarios()
            };
            return View(viewModel);
        }

        RequisicaoEntrada requisicaoEntrada = new RequisicaoEntrada(
            produto,
            viewModel.Quantidade,
            funcionario,
            viewModel.Tipo,
            viewModel.NumeroNotaFiscal);

        foreach (string erro in requisicaoEntrada.Validar())
            ModelState.AddModelError(string.Empty, erro);

        if (!ModelState.IsValid)
        {
            produto.RemoverRequisicao(requisicaoEntrada);
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

    [HttpGet]
    public ActionResult Editar(int id)
    {
        RequisicaoEntrada? requisicao = repositorio.SelecionarPorId(id);

        if (requisicao == null)
            return NotFound();

        EditarRequisicaoEntradaViewModel viewModel = new EditarRequisicaoEntradaViewModel(
            id,
            requisicao.Produto.Id,
            requisicao.Funcionario.Id,
            requisicao.Quantidade,
            requisicao.Tipo,
            requisicao.NumeroNotaFiscal)
        {
            Produtos = ObterProdutos(),
            Funcionarios = ObterFuncionarios()
        };

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Editar(EditarRequisicaoEntradaViewModel viewModel)
    {
        RequisicaoEntrada? original = repositorio.SelecionarPorId(viewModel.Id);

        if (original == null)
            return NotFound();

        Produto? produto = repositorioProduto.SelecionarPorId(viewModel.ProdutoId);
        Funcionario? funcionario = repositorioFuncionario.SelecionarPorId(viewModel.FuncionarioId);

        if (produto == null)
            ModelState.AddModelError(nameof(viewModel.ProdutoId), "Selecione um produto.");

        if (funcionario == null)
            ModelState.AddModelError(nameof(viewModel.FuncionarioId), "Selecione um funcionário.");

        original.Produto.RemoverRequisicao(original);

        if (produto != null && funcionario != null)
        {
            RequisicaoEntrada atualizada = new RequisicaoEntrada(
                produto,
                viewModel.Quantidade,
                funcionario,
                viewModel.Tipo,
                viewModel.NumeroNotaFiscal);

            foreach (string erro in atualizada.Validar())
                ModelState.AddModelError(string.Empty, erro);

            if (ModelState.IsValid)
            {
                atualizada.Id = viewModel.Id;
                atualizada.Data = original.Data;

                if (repositorio.Editar(viewModel.Id, atualizada))
                    return RedirectToAction(nameof(Listar));

                return NotFound();
            }

            produto.RemoverRequisicao(atualizada);
        }

        original.Produto.RegistrarRequisicao(original);

        viewModel = viewModel with
        {
            Produtos = ObterProdutos(),
            Funcionarios = ObterFuncionarios()
        };

        return View(viewModel);
    }

    [HttpGet]
    public ActionResult Excluir(int id)
    {
        RequisicaoEntrada? requisicao = repositorio.SelecionarPorId(id);

        if (requisicao == null)
            return NotFound();

        return View(requisicao);
    }

    [HttpPost]
    [ActionName("Excluir")]
    public ActionResult ConfirmarExclusao(int id)
    {
        RequisicaoEntrada? requisicao = repositorio.SelecionarPorId(id);

        if (requisicao == null)
            return NotFound();

        requisicao.Produto.RemoverRequisicao(requisicao);

        if (!repositorio.Excluir(id))
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }

    private List<ProdutoRequisicaoEntradaViewModel> ObterProdutos()
    {
        List<ProdutoRequisicaoEntradaViewModel> viewModels = [];

        foreach (Produto produto in repositorioProduto.SelecionarTodos())
            viewModels.Add(new ProdutoRequisicaoEntradaViewModel(produto.Id, produto.Nome));

        return viewModels;
    }

    private List<FuncionarioRequisicaoEntradaViewModel> ObterFuncionarios()
    {
        List<FuncionarioRequisicaoEntradaViewModel> viewModels = [];

        foreach (Funcionario funcionario in repositorioFuncionario.SelecionarTodos())
            viewModels.Add(new FuncionarioRequisicaoEntradaViewModel(funcionario.Id, funcionario.Nome));

        return viewModels;
    }
}
