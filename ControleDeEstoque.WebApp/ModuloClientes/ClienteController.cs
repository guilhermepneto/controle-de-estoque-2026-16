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

        foreach (Cliente cliente in repositorio.SelecionarTodos())
        {
            viewModels.Add(new ListarClienteViewModel(
                cliente.Id,
                cliente.Nome,
                cliente.Telefone,
                cliente.Email,
                cliente.Documento,
                cliente.TipoDocumento
            ));
        }

        return View(viewModels);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        return View(new CadastrarClienteViewModel(
            string.Empty, string.Empty, string.Empty, string.Empty, TipoDocumentoCliente.CPF));
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarClienteViewModel viewModel)
    {
        Cliente cliente = new(
            viewModel.Nome,
            viewModel.Telefone,
            viewModel.Email,
            viewModel.Cpf,
            viewModel.TipoDocumento
        );

        foreach (string erro in cliente.Validar())
            ModelState.AddModelError(string.Empty, erro);

        if (!ModelState.IsValid)
            return View(viewModel);

        repositorio.Cadastrar(cliente);
        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(int id)
    {
        Cliente? cliente = repositorio.SelecionarPorId(id);

        if (cliente == null)
            return NotFound();

        return View(new EditarClienteViewModel(
            id,
            cliente.Nome,
            cliente.Telefone,
            cliente.Email,
            cliente.Documento,
            cliente.TipoDocumento
        ));
    }

    [HttpPost]
    public ActionResult Editar(EditarClienteViewModel viewModel)
    {
        Cliente clienteAtualizado = new(
            viewModel.Nome,
            viewModel.Telefone,
            viewModel.Email,
            viewModel.Cpf,
            viewModel.TipoDocumento
        );

        foreach (string erro in clienteAtualizado.Validar())
            ModelState.AddModelError(string.Empty, erro);

        if (!ModelState.IsValid)
            return View(viewModel);

        if (!repositorio.Editar(viewModel.Id, clienteAtualizado))
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Excluir(int id)
    {
        Cliente? cliente = repositorio.SelecionarPorId(id);

        if (cliente == null)
            return NotFound();

        return View(new ExcluirClienteViewModel(id, cliente.Nome));
    }

    [HttpPost]
    [ActionName("Excluir")]
    public ActionResult ConfirmarExclusao(int id)
    {
        if (!repositorio.Excluir(id))
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }
}
