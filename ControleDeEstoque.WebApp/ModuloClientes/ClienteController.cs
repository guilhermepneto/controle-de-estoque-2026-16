using ControleDeEstoque.WebApp.Compartilhado.Arquivos;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeEstoque.WebApp.ModuloClientes;

public class ClienteController : Controller
{
    private readonly RepositorioClienteEmArquivo repositorio;

    public ClienteController(RepositorioClienteEmArquivo repositorio)
    {
        this.repositorio = repositorio;
    }

    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarClienteViewModel> viewModels = [];

        foreach (Cliente p in repositorio.SelecionarTodos())
        {
            ListarClienteViewModel viewModel = new ListarClienteViewModel(
                p.Id,
                p.Nome,
                p.Telefone,
                p.Email
            );

            viewModels.Add(viewModel);
        }

        return View(viewModels);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarClienteViewModel viewModel = new CadastrarClienteViewModel(
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
        );

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarClienteViewModel viewModel)
    {
        Cliente cliente = new Cliente(
            viewModel.Nome,
            viewModel.Telefone,
            viewModel.Email,
            viewModel.Cpf
        );

        repositorio.Cadastrar(cliente);

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(int id)
    {
        Cliente? cliente = repositorio.SelecionarPorId(id);

        if (cliente == null)
            return NotFound();

        EditarClienteViewModel viewModel = new EditarClienteViewModel(
            id,
            cliente.Nome,
            cliente.Telefone,
            cliente.Email,
            cliente.Cpf
        );

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Editar(EditarClienteViewModel viewModel)
    {
        Cliente clienteAtualizado = new Cliente(
            viewModel.Nome,
            viewModel.Telefone,
            viewModel.Email,
            viewModel.Cpf
        );

        bool conseguiuEditar = repositorio.Editar(viewModel.Id, clienteAtualizado);

        if (!conseguiuEditar)
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Excluir(int id)
    {
        Cliente? cliente = repositorio.SelecionarPorId(id);

        if (cliente == null)
            return NotFound();

        ExcluirClienteViewModel viewModel = new ExcluirClienteViewModel(
            id,
            cliente.Nome
        );

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("Excluir")]
    public ActionResult ConfirmarExclusao(int id)
    {
        bool conseguiuExcluir = repositorio.Excluir(id);

        if (!conseguiuExcluir)
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }
}
