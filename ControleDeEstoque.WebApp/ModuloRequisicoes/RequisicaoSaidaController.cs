using ControleDeEstoque.WebApp.Compartilhado.Arquivos;
using ControleDeEstoque.WebApp.ModuloProdutos;
using ControleDeEstoque.WebApp.ModuloClientes;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public class RequisicaoSaidaController : Controller
{
    private readonly RepositorioRequisicaoSaidaEmArquivo repositorio;
    private readonly RepositorioClienteEmArquivo repositorioCliente;
    private readonly RepositorioProdutoEmArquivo repositorioProduto;

    public RequisicaoSaidaController(
        RepositorioRequisicaoSaidaEmArquivo repositorio,
        RepositorioClienteEmArquivo repositorioCliente,
        RepositorioProdutoEmArquivo repositorioProduto)
    {
        this.repositorio = repositorio;
        this.repositorioCliente = repositorioCliente;
        this.repositorioProduto = repositorioProduto;
    }

    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarRequisicaoSaidaViewModel> viewModels = [];

        foreach (RequisicaoSaida requisicao in repositorio.SelecionarTodos())
        {
            List<ListarProdutoPrescritoRequisicaoSaidaViewModel> produtosPrescritosVMs = [];

            foreach (ProdutoPrescrito prescrito in requisicao.ProdutosPrescritos)
            {
                produtosPrescritosVMs.Add(new ListarProdutoPrescritoRequisicaoSaidaViewModel(
                    prescrito.Produto.Id,
                    prescrito.Produto.Nome,
                    prescrito.Quantidade));
            }

            viewModels.Add(new ListarRequisicaoSaidaViewModel(
                requisicao.Id,
                requisicao.Cliente.Nome,
                requisicao.Data,
                produtosPrescritosVMs));
        }

        return View(viewModels);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarRequisicaoSaidaViewModel viewModel = new CadastrarRequisicaoSaidaViewModel(0) with
        {
            Clientes = ObterClientes(),
            ProdutosPrescritos = ObterProdutos()
        };

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarRequisicaoSaidaViewModel viewModel)
    {
        Cliente? cliente = repositorioCliente.SelecionarPorId(viewModel.ClienteId);

        if (cliente == null)
        {
            ModelState.AddModelError(nameof(viewModel.ClienteId), "Selecione um cliente.");
            viewModel = viewModel with
            {
                Clientes = ObterClientes(),
                ProdutosPrescritos = ObterProdutos(viewModel.ProdutosPrescritos)
            };
            return View(viewModel);
        }

        List<ProdutoPrescrito> produtosPrescritos = [];

        foreach (ProdutoPrescritoRequisicaoSaidaViewModel produtoModel in viewModel.ProdutosPrescritos ?? [])
        {
            if (!produtoModel.Selecionado)
                continue;

            Produto? produto = repositorioProduto.SelecionarPorId(produtoModel.ProdutoId);

            if (produto == null)
            {
                ModelState.AddModelError(string.Empty, "Um dos produtos selecionados não existe mais.");
                continue;
            }

            produtosPrescritos.Add(new ProdutoPrescrito(produto, produtoModel.Quantidade));
        }

        RequisicaoSaida requisicao = new RequisicaoSaida(cliente, produtosPrescritos);
        List<string> erros = requisicao.Validar();

        foreach (string erro in erros)
            ModelState.AddModelError(string.Empty, erro);

        if (!ModelState.IsValid)
        {
            foreach (ProdutoPrescrito produto in requisicao.ProdutosPrescritos)
                produto.Produto.RemoverRequisicaoSaida(requisicao);

            viewModel = viewModel with
            {
                Clientes = ObterClientes(),
                ProdutosPrescritos = ObterProdutos(viewModel.ProdutosPrescritos)
            };

            return View(viewModel);
        }

        repositorio.Cadastrar(requisicao);
        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(int id)
    {
        RequisicaoSaida? requisicao = repositorio.SelecionarPorId(id);

        if (requisicao == null)
            return NotFound();

        EditarRequisicaoSaidaViewModel viewModel = new EditarRequisicaoSaidaViewModel(id, requisicao.Cliente.Id)
        {
            Clientes = ObterClientes(),
            ProdutosPrescritos = ObterProdutosParaEdicao(requisicao)
        };

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Editar(EditarRequisicaoSaidaViewModel viewModel)
    {
        RequisicaoSaida? requisicaoOriginal = repositorio.SelecionarPorId(viewModel.Id);

        if (requisicaoOriginal == null)
            return NotFound();

        Cliente? cliente = repositorioCliente.SelecionarPorId(viewModel.ClienteId);

        if (cliente == null)
            ModelState.AddModelError(nameof(viewModel.ClienteId), "Selecione um cliente.");

        foreach (ProdutoPrescrito antigo in requisicaoOriginal.ProdutosPrescritos)
            antigo.Produto.RemoverRequisicaoSaida(requisicaoOriginal);

        List<ProdutoPrescrito> produtosPrescritos = [];

        foreach (ProdutoPrescritoRequisicaoSaidaViewModel produtoModel in viewModel.ProdutosPrescritos ?? [])
        {
            if (!produtoModel.Selecionado)
                continue;

            Produto? produto = repositorioProduto.SelecionarPorId(produtoModel.ProdutoId);

            if (produto == null)
            {
                ModelState.AddModelError(string.Empty, "Um dos produtos selecionados não existe mais.");
                continue;
            }

            produtosPrescritos.Add(new ProdutoPrescrito(produto, produtoModel.Quantidade));
        }

        if (cliente != null)
        {
            RequisicaoSaida requisicaoAtualizada = new RequisicaoSaida(cliente, produtosPrescritos);
            List<string> erros = requisicaoAtualizada.Validar();

            foreach (string erro in erros)
                ModelState.AddModelError(string.Empty, erro);

            if (ModelState.IsValid)
            {
                requisicaoAtualizada.Id = viewModel.Id;
                requisicaoAtualizada.Data = requisicaoOriginal.Data;

                if (repositorio.Editar(viewModel.Id, requisicaoAtualizada))
                    return RedirectToAction(nameof(Listar));

                return NotFound();
            }

            foreach (ProdutoPrescrito produto in requisicaoAtualizada.ProdutosPrescritos)
                produto.Produto.RemoverRequisicaoSaida(requisicaoAtualizada);
        }

        foreach (ProdutoPrescrito antigo in requisicaoOriginal.ProdutosPrescritos)
            antigo.Produto.RegistrarRequisicaoSaida(requisicaoOriginal);

        viewModel = viewModel with
        {
            Clientes = ObterClientes(),
            ProdutosPrescritos = ObterProdutos(viewModel.ProdutosPrescritos)
        };

        return View(viewModel);
    }

    [HttpGet]
    public ActionResult Excluir(int id)
    {
        RequisicaoSaida? requisicao = repositorio.SelecionarPorId(id);

        if (requisicao == null)
            return NotFound();

        return View(requisicao);
    }

    [HttpPost]
    [ActionName("Excluir")]
    public ActionResult ConfirmarExclusao(int id)
    {
        RequisicaoSaida? requisicao = repositorio.SelecionarPorId(id);

        if (requisicao == null)
            return NotFound();

        foreach (ProdutoPrescrito produto in requisicao.ProdutosPrescritos)
            produto.Produto.RemoverRequisicaoSaida(requisicao);

        if (!repositorio.Excluir(id))
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }

    private List<ClienteRequisicaoSaidaViewModel> ObterClientes()
    {
        List<ClienteRequisicaoSaidaViewModel> viewModels = [];

        foreach (Cliente cliente in repositorioCliente.SelecionarTodos())
            viewModels.Add(new ClienteRequisicaoSaidaViewModel(cliente.Id, cliente.Nome));

        return viewModels;
    }

    private List<ProdutoPrescritoRequisicaoSaidaViewModel> ObterProdutos(
        List<ProdutoPrescritoRequisicaoSaidaViewModel>? valoresEnviados = null)
    {
        Dictionary<int, ProdutoPrescritoRequisicaoSaidaViewModel> valoresPorProduto = [];

        if (valoresEnviados != null)
        {
            foreach (ProdutoPrescritoRequisicaoSaidaViewModel valor in valoresEnviados)
                valoresPorProduto[valor.ProdutoId] = valor;
        }

        List<ProdutoPrescritoRequisicaoSaidaViewModel> viewModels = [];

        foreach (Produto produto in repositorioProduto.SelecionarTodos())
        {
            valoresPorProduto.TryGetValue(produto.Id, out ProdutoPrescritoRequisicaoSaidaViewModel? valor);

            viewModels.Add(new ProdutoPrescritoRequisicaoSaidaViewModel(
                produto.Id,
                produto.Nome,
                produto.QuantidadeEmEstoque,
                valor?.Selecionado ?? false,
                valor?.Quantidade ?? 0));
        }

        return viewModels;
    }

    private List<ProdutoPrescritoRequisicaoSaidaViewModel> ObterProdutosParaEdicao(RequisicaoSaida requisicao)
    {
        Dictionary<int, int> quantidades = [];

        foreach (ProdutoPrescrito produto in requisicao.ProdutosPrescritos)
            quantidades[produto.Produto.Id] = produto.Quantidade;

        List<ProdutoPrescritoRequisicaoSaidaViewModel> viewModels = [];

        foreach (Produto produto in repositorioProduto.SelecionarTodos())
        {
            bool selecionado = quantidades.ContainsKey(produto.Id);
            int quantidade = quantidades.TryGetValue(produto.Id, out int valor) ? valor : 0;

            viewModels.Add(new ProdutoPrescritoRequisicaoSaidaViewModel(
                produto.Id,
                produto.Nome,
                produto.QuantidadeEmEstoque,
                selecionado,
                quantidade));
        }

        return viewModels;
    }
}
