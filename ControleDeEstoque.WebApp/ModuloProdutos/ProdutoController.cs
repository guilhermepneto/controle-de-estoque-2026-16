using ControleDeEstoque.WebApp.Compartilhado.Arquivos;
using ControleDeEstoque.WebApp.ModuloFornecedores;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeEstoque.WebApp.ModuloProdutos;

public sealed class ProdutoController : Controller
{
    private readonly RepositorioProdutoEmArquivo repositorioProduto;
    private readonly RepositorioFornecedorEmArquivo repositorioFornecedor;

    public ProdutoController()
    {
        ContextoJson contexto = new ContextoJson();

        contexto.Carregar();

        repositorioProduto = new RepositorioProdutoEmArquivo(contexto);
        repositorioFornecedor = new RepositorioFornecedorEmArquivo(contexto);
    }

    [HttpGet]
    public ActionResult Listar()
    {
        List<Produto> produtos = repositorioProduto.SelecionarTodos();

        List<ListarProdutoViewModel> viewModels = [];

        foreach (Produto med in produtos)
        {
            ListarProdutoViewModel viewModel = new ListarProdutoViewModel(
                med.Id,
                med.Nome,
                med.Descricao,
                med.Fornecedor.Nome,
                med.QuantidadeEmEstoque
            );

            viewModels.Add(viewModel);
        }

        return View(viewModels);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarProdutoViewModel viewModel = new CadastrarProdutoViewModel(
           string.Empty,
           string.Empty,
           0
       ) with
        { Fornecedores = ObterFornecedores() };

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarProdutoViewModel viewModel)
    {
        Fornecedor? fornecedor = repositorioFornecedor.SelecionarPorId(viewModel.FornecedorId);

        if (fornecedor == null)
            return NotFound();

        Produto produto = new Produto(viewModel.Nome, viewModel.Descricao, fornecedor);

        repositorioProduto.Cadastrar(produto);

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(int id)
    {
        Produto? produto = repositorioProduto.SelecionarPorId(id);

        if (produto == null)
            return NotFound();

        EditarProdutoViewModel viewModel = new EditarProdutoViewModel(
            id,
            produto.Nome,
            produto.Descricao,
            produto.Fornecedor.Id
        ) with
        {
            Fornecedores = ObterFornecedores()
        };

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Editar(EditarProdutoViewModel viewModel)
    {
        Fornecedor? fornecedor = repositorioFornecedor.SelecionarPorId(viewModel.FornecedorId);

        if (fornecedor == null)
            return NotFound();

        Produto produtoAtualizado = new Produto(viewModel.Nome, viewModel.Descricao, fornecedor); ;

        bool conseguiuEditar = repositorioProduto.Editar(viewModel.Id, produtoAtualizado);

        if (!conseguiuEditar)
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Excluir(int id)
    {
        Produto? produto = repositorioProduto.SelecionarPorId(id);

        if (produto == null)
            return NotFound();

        return View(produto);
    }

    [HttpPost]
    [ActionName("Excluir")]
    public ActionResult ConfirmarExclusao(int id)
    {
        bool conseguiuExcluir = repositorioProduto.Excluir(id);

        if (!conseguiuExcluir)
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }

    private List<FornecedorProdutoViewModel> ObterFornecedores()
    {
        List<Fornecedor> fornecedores = repositorioFornecedor.SelecionarTodos();

        List<FornecedorProdutoViewModel> fornecedoresVms = [];

        foreach (Fornecedor f in fornecedores)
        {
            FornecedorProdutoViewModel fornecedorVm = new FornecedorProdutoViewModel(
                f.Id,
                f.Nome
            );

            fornecedoresVms.Add(fornecedorVm);
        }

        return fornecedoresVms;
    }
}
