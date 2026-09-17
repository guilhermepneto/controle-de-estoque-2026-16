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
    public ActionResult Listar(string? pesquisa)
    {
        pesquisa = pesquisa?.Trim();
        List<ListarRequisicaoEntradaViewModel> viewModels = new List<ListarRequisicaoEntradaViewModel>();

        foreach (RequisicaoEntrada requisicao in repositorio.SelecionarTodos())
        {
            if (!string.IsNullOrWhiteSpace(pesquisa) && !CorrespondePesquisa(requisicao, pesquisa))
                continue;

            viewModels.Add(new ListarRequisicaoEntradaViewModel(
                requisicao.Id,
                requisicao.Produto?.Nome ?? "Produto não encontrado",
                requisicao.Funcionario?.Nome ?? "Funcionário não encontrado",
                requisicao.Quantidade,
                requisicao.Data,
                requisicao.Tipo,
                requisicao.NumeroNotaFiscal));
        }

        ViewBag.Pesquisa = pesquisa;
        return View(viewModels);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarRequisicaoEntradaViewModel viewModel =
            new CadastrarRequisicaoEntradaViewModel(0, 0, 0)
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
            return RetornarCadastroComListas(viewModel);

        RequisicaoEntrada requisicao = new RequisicaoEntrada(
            produto,
            viewModel.Quantidade,
            funcionario,
            viewModel.Tipo,
            viewModel.NumeroNotaFiscal);

        foreach (string erro in requisicao.Validar())
            ModelState.AddModelError(string.Empty, erro);

        if (ModelState.IsValid)
        {
            repositorio.Cadastrar(requisicao);
            return RedirectToAction(nameof(Listar));
        }

        produto.RemoverRequisicao(requisicao);
        return RetornarCadastroComListas(viewModel);
    }

    [HttpGet]
    public ActionResult Editar(int id)
    {
        RequisicaoEntrada? requisicao = repositorio.SelecionarPorId(id);

        if (requisicao == null)
            return NotFound();

        EditarRequisicaoEntradaViewModel viewModel =
            new EditarRequisicaoEntradaViewModel(
                requisicao.Id,
                requisicao.Produto?.Id ?? 0,
                requisicao.Funcionario?.Id ?? 0,
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

        Produto produtoOriginal = original.Produto;
        produtoOriginal.RemoverRequisicao(original);

        if (produto != null && funcionario != null)
        {
            // Cria a versão nova sem registrá-la no Produto antes da validação.
            RequisicaoEntrada atualizada = new RequisicaoEntrada
            {
                Id = original.Id,
                Produto = produto,
                Quantidade = viewModel.Quantidade,
                Funcionario = funcionario,
                Tipo = viewModel.Tipo,
                NumeroNotaFiscal = viewModel.NumeroNotaFiscal,
                Data = original.Data
            };

            foreach (string erro in atualizada.Validar())
                ModelState.AddModelError(string.Empty, erro);

            if (ModelState.IsValid)
            {
                original.Atualizar(atualizada);
                original.Produto.RegistrarRequisicao(original);

                if (repositorio.Editar(viewModel.Id, original))
                    return RedirectToAction(nameof(Listar));

                original.Produto.RemoverRequisicao(original);
                produtoOriginal.RegistrarRequisicao(original);
                return NotFound();
            }
        }

        produtoOriginal.RegistrarRequisicao(original);

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

        Produto produto = requisicao.Produto;
        produto.RemoverRequisicao(requisicao);

        if (!repositorio.Excluir(id))
        {
            produto.RegistrarRequisicao(requisicao);
            return NotFound();
        }

        return RedirectToAction(nameof(Listar));
    }

    private bool CorrespondePesquisa(RequisicaoEntrada requisicao, string pesquisa)
    {
        if (requisicao.Id.ToString().Contains(pesquisa, StringComparison.OrdinalIgnoreCase))
            return true;

        if (requisicao.Data.ToString("dd/MM/yyyy").Contains(pesquisa, StringComparison.OrdinalIgnoreCase))
            return true;

        if ((requisicao.Produto?.Nome ?? string.Empty).Contains(pesquisa, StringComparison.OrdinalIgnoreCase))
            return true;

        if ((requisicao.Funcionario?.Nome ?? string.Empty).Contains(pesquisa, StringComparison.OrdinalIgnoreCase))
            return true;

        if ((requisicao.NumeroNotaFiscal ?? string.Empty).Contains(pesquisa, StringComparison.OrdinalIgnoreCase))
            return true;

        string tipo = requisicao.Tipo == TipoEntrada.Devolucao ? "Devolução" : "Nota Fiscal";

        return tipo.Contains(pesquisa, StringComparison.OrdinalIgnoreCase);
    }

    private ActionResult RetornarCadastroComListas(CadastrarRequisicaoEntradaViewModel viewModel)
    {
        viewModel = viewModel with
        {
            Produtos = ObterProdutos(),
            Funcionarios = ObterFuncionarios()
        };

        return View("Cadastrar", viewModel);
    }

    private List<ProdutoRequisicaoEntradaViewModel> ObterProdutos()
    {
        List<ProdutoRequisicaoEntradaViewModel> viewModels =
            new List<ProdutoRequisicaoEntradaViewModel>();

        foreach (Produto produto in repositorioProduto.SelecionarTodos())
            viewModels.Add(new ProdutoRequisicaoEntradaViewModel(produto.Id, produto.Nome));

        return viewModels;
    }

    private List<FuncionarioRequisicaoEntradaViewModel> ObterFuncionarios()
    {
        List<FuncionarioRequisicaoEntradaViewModel> viewModels =
            new List<FuncionarioRequisicaoEntradaViewModel>();

        foreach (Funcionario funcionario in repositorioFuncionario.SelecionarTodos())
            viewModels.Add(new FuncionarioRequisicaoEntradaViewModel(funcionario.Id, funcionario.Nome));

        return viewModels;
    }
}
