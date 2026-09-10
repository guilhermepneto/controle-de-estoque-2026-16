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
                ListarProdutoPrescritoRequisicaoSaidaViewModel prescritoVm = new(
                    prescrito.Produto.Id,
                    prescrito.Produto.Nome,
                    prescrito.Quantidade
                );

                produtosPrescritosVMs.Add(prescritoVm);
            }

            ListarRequisicaoSaidaViewModel viewModel = new ListarRequisicaoSaidaViewModel(
                requisicao.Id,
                requisicao.Cliente.Nome,
                requisicao.Data,
                produtosPrescritosVMs
            );

            viewModels.Add(viewModel);
        }

        return View(viewModels);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarRequisicaoSaidaViewModel viewModel = new CadastrarRequisicaoSaidaViewModel(
            0
        ) with
        { Clientes = ObterClientes(), ProdutosPrescritos = ObterProdutos() };

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarRequisicaoSaidaViewModel viewModel)
    {
        Cliente? cliente = repositorioCliente.SelecionarPorId(viewModel.ClienteId);

        if (cliente == null)
            return NotFound();

        List<ProdutoPrescritoRequisicaoSaidaViewModel> produtosModel =
            viewModel.ProdutosPrescritos ?? [];

        List<ProdutoPrescrito> produtosPrescritos = [];

        foreach (ProdutoPrescritoRequisicaoSaidaViewModel produtoModel in produtosModel)
        {
            if (!produtoModel.Selecionado)
                continue;

            Produto? produto = repositorioProduto.SelecionarPorId(produtoModel.ProdutoId);
            produtosPrescritos.Add(
                new ProdutoPrescrito(produto!, produtoModel.Quantidade));
        }

        RequisicaoSaida requisicao = new RequisicaoSaida(cliente, produtosPrescritos);

        List<string> erros = requisicao.Validar();

        if (erros.Count > 0)
        {
            ModelState.AddModelError(string.Empty, erros.First());
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

    private List<ClienteRequisicaoSaidaViewModel> ObterClientes()
    {
        List<ClienteRequisicaoSaidaViewModel> viewModels = [];

        foreach (Cliente cliente in repositorioCliente.SelecionarTodos())
        {
            ClienteRequisicaoSaidaViewModel viewModel = new ClienteRequisicaoSaidaViewModel(
                cliente.Id,
                cliente.Nome
            );

            viewModels.Add(viewModel);
        }

        return viewModels;
    }

    private List<ProdutoPrescritoRequisicaoSaidaViewModel> ObterProdutos(
        List<ProdutoPrescritoRequisicaoSaidaViewModel>? valoresEnviados = null
    )
    {
        // Id 1 = Produto Prescrito { Nome = Paracetamol ...}
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
                valor?.Quantidade ?? 0
            ));
        }

        return viewModels;
    }
}
