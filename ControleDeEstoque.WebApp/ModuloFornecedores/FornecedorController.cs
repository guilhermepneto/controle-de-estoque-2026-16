namespace ControleDeEstoque.WebApp.ModuloFornecedores;

using ControleDeEstoque.WebApp.Compartilhado.Arquivos;
using Microsoft.AspNetCore.Mvc;

public sealed class FornecedorController : Controller
{
    private readonly RepositorioFornecedorEmArquivo repositorioFornecedor;

    public FornecedorController(RepositorioFornecedorEmArquivo repositorioFornecedor)
    {
        this.repositorioFornecedor = repositorioFornecedor;
    }

    [HttpGet]
    public ActionResult Listar()
    {
        List<Fornecedor> fornecedores = repositorioFornecedor.SelecionarTodos();

        List<ListarFornecedorViewModel> viewModels = new List<ListarFornecedorViewModel>();

        foreach (Fornecedor f in fornecedores)
        {
            ListarFornecedorViewModel vm = new ListarFornecedorViewModel(
                f.Id,
                f.Nome,
                f.Telefone,
                f.Cnpj
            );

            viewModels.Add(vm);
        }

        return View(viewModels);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarFornecedorViewModel cadastrarVm)
    {
        Fornecedor fornecedor = new Fornecedor(
            cadastrarVm.Nome ?? string.Empty,
            cadastrarVm.Telefone ?? string.Empty,
            cadastrarVm.Cnpj ?? string.Empty
            );

        if (!ModelState.IsValid)
            return View(cadastrarVm);

        repositorioFornecedor.Cadastrar(fornecedor);


        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(int id)
    {
        Fornecedor? fornecedor = repositorioFornecedor.SelecionarPorId(id);

        if (fornecedor == null)
            return NotFound();

        EditarFornecedorViewModel viewModel = new EditarFornecedorViewModel(
          id,
          fornecedor.Nome,
          fornecedor.Telefone,
          fornecedor.Cnpj
        );

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Editar(EditarFornecedorViewModel editarVm)
    {
        Fornecedor fornecedorAtualizado = new Fornecedor(
            editarVm.Nome,
            editarVm.Telefone,
            editarVm.Cnpj
            );

        if (!ModelState.IsValid)
            return View(editarVm);

        bool conseguiuEditar = repositorioFornecedor.Editar(editarVm.Id, fornecedorAtualizado);

        if (!conseguiuEditar)
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Excluir(int id)
    {
        Fornecedor? fornecedor = repositorioFornecedor.SelecionarPorId(id);

        if (fornecedor == null)
            return NotFound();

        ExcluirFornecedorViewModel viewModel = new ExcluirFornecedorViewModel(
            id,
            fornecedor.Nome
        );

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Excluir(ExcluirFornecedorViewModel excluirVm)
    {
        bool conseguiuExcluir = repositorioFornecedor.Excluir(excluirVm.Id);

        if (!conseguiuExcluir)
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }

}
