using ControleDeEstoque.WebApp.ModuloFornecedores;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeEstoque.WebApp.ModuloProdutos;

public sealed class ProdutoController : Controller
{
    private readonly RepositorioProdutoEmArquivo repositorioProduto;
    private readonly RepositorioFornecedorEmArquivo repositorioFornecedor;

    public ProdutoController(
        RepositorioProdutoEmArquivo repositorioProduto,
        RepositorioFornecedorEmArquivo repositorioFornecedor)
    {
        this.repositorioProduto = repositorioProduto;
        this.repositorioFornecedor = repositorioFornecedor;
    }

    [HttpGet]
    public ActionResult Listar(CategoriaProduto? categoria = null)
    {
        List<ListarProdutoViewModel> viewModels = [];

        foreach (Produto produto in repositorioProduto.SelecionarTodos())
        {
            if (categoria.HasValue && produto.Categoria != categoria.Value)
                continue;

            viewModels.Add(new ListarProdutoViewModel(
                produto.Id,
                produto.Nome,
                produto.Descricao,
                produto.Fornecedor.Nome,
                produto.QuantidadeEmEstoque,
                produto.Categoria
            ));
        }

        ViewBag.Categoria = categoria;
        ViewBag.Titulo = categoria switch
        {
            CategoriaProduto.Tinta => "Tintas",
            CategoriaProduto.Toner => "Toners",
            _ => "Listagem de Produtos"
        };

        return View(viewModels);
    }
    [HttpGet]
    public ActionResult Cadastrar(CategoriaProduto categoria = CategoriaProduto.Geral)
    {
        CadastrarProdutoViewModel viewModel = new(
            string.Empty,
            string.Empty,
            0,
            categoria
        )
        {
            Fornecedores = ObterFornecedores()
        };

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarProdutoViewModel viewModel)
    {
        Fornecedor? fornecedor = repositorioFornecedor.SelecionarPorId(viewModel.FornecedorId);

        if (fornecedor == null)
            return NotFound();

        Produto produto = new(
            viewModel.Nome,
            viewModel.Descricao,
            fornecedor,
            viewModel.Categoria
        );

        foreach (string erro in produto.Validar())
            ModelState.AddModelError(string.Empty, erro);

        if (!ModelState.IsValid)
        {
            viewModel = viewModel with { Fornecedores = ObterFornecedores() };
            return View(viewModel);
        }

        repositorioProduto.Cadastrar(produto);

        return RedirectToAction(nameof(Listar), new { categoria = viewModel.Categoria == CategoriaProduto.Geral ? (CategoriaProduto?)null : viewModel.Categoria });
    }

    [HttpGet]
    public ActionResult Editar(int id)
    {
        Produto? produto = repositorioProduto.SelecionarPorId(id);

        if (produto == null)
            return NotFound();

        EditarProdutoViewModel viewModel = new(
            id,
            produto.Nome,
            produto.Descricao,
            produto.Fornecedor.Id,
            produto.Categoria
        )
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

        Produto produtoAtualizado = new(
            viewModel.Nome,
            viewModel.Descricao,
            fornecedor,
            viewModel.Categoria
        );

        foreach (string erro in produtoAtualizado.Validar())
            ModelState.AddModelError(string.Empty, erro);

        if (!ModelState.IsValid)
        {
            viewModel = viewModel with { Fornecedores = ObterFornecedores() };
            return View(viewModel);
        }

        if (!repositorioProduto.Editar(viewModel.Id, produtoAtualizado))
            return NotFound();

        return RedirectToAction(nameof(Listar), new
        {
            categoria = viewModel.Categoria == CategoriaProduto.Geral
                ? (CategoriaProduto?)null
                : viewModel.Categoria
        });
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
        if (!repositorioProduto.Excluir(id))
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }

    private List<FornecedorProdutoViewModel> ObterFornecedores()
    {
        List<FornecedorProdutoViewModel> fornecedoresVms = [];

        foreach (Fornecedor fornecedor in repositorioFornecedor.SelecionarTodos())
            fornecedoresVms.Add(new FornecedorProdutoViewModel(fornecedor.Id, fornecedor.Nome));

        return fornecedoresVms;
    }
}
