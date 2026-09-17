using ControleDeEstoque.WebApp.Compartilhado.Arquivos;
using ControleDeEstoque.WebApp.ModuloClientes;
using ControleDeEstoque.WebApp.ModuloProdutos;
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
    public ActionResult Listar(string? pesquisa)
    {
        pesquisa = pesquisa?.Trim();

        List<ListarRequisicaoSaidaViewModel> viewModels = new List<ListarRequisicaoSaidaViewModel>();

        foreach (RequisicaoSaida requisicao in repositorio.SelecionarTodos())
        {
            if (!string.IsNullOrWhiteSpace(pesquisa) && !CorrespondePesquisa(requisicao, pesquisa))
                continue;

            List<ListarProdutoPrescritoRequisicaoSaidaViewModel> produtos =
                new List<ListarProdutoPrescritoRequisicaoSaidaViewModel>();

            foreach (ProdutoPrescrito prescrito in requisicao.ProdutosPrescritos)
            {
                if (prescrito.Produto == null)
                    continue;

                produtos.Add(new ListarProdutoPrescritoRequisicaoSaidaViewModel(
                    prescrito.Produto.Id,
                    prescrito.Produto.Nome,
                    prescrito.Quantidade));
            }

            viewModels.Add(new ListarRequisicaoSaidaViewModel(
                requisicao.Id,
                requisicao.Cliente?.Nome ?? "Cliente não encontrado",
                requisicao.Data,
                produtos));
        }

        ViewBag.Pesquisa = pesquisa;
        return View(viewModels);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarRequisicaoSaidaViewModel viewModel =
            new CadastrarRequisicaoSaidaViewModel(0)
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
            ModelState.AddModelError(nameof(viewModel.ClienteId), "Selecione um cliente.");

        List<ProdutoPrescrito> produtosPrescritos = CriarProdutosSelecionados(
            viewModel.ProdutosPrescritos,
            out List<Produto> produtosRegistrados);

        if (cliente != null)
        {
            RequisicaoSaida requisicao = new RequisicaoSaida(cliente, produtosPrescritos);

            foreach (string erro in requisicao.Validar())
                ModelState.AddModelError(string.Empty, erro);

            if (ModelState.IsValid)
            {
                repositorio.Cadastrar(requisicao);
                return RedirectToAction(nameof(Listar));
            }

            RemoverRequisicaoDosProdutos(requisicao);
        }

        viewModel = viewModel with
        {
            Clientes = ObterClientes(),
            ProdutosPrescritos = ObterProdutos(viewModel.ProdutosPrescritos)
        };

        return View(viewModel);
    }

    [HttpGet]
    public ActionResult Editar(int id)
    {
        RequisicaoSaida? requisicao = repositorio.SelecionarPorId(id);

        if (requisicao == null)
            return NotFound();

        EditarRequisicaoSaidaViewModel viewModel =
            new EditarRequisicaoSaidaViewModel(
                requisicao.Id,
                requisicao.Cliente?.Id ?? 0)
            {
                Clientes = ObterClientes(),
                ProdutosPrescritos = ObterProdutosParaEdicao(requisicao)
            };

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Editar(EditarRequisicaoSaidaViewModel viewModel)
    {
        RequisicaoSaida? original = repositorio.SelecionarPorId(viewModel.Id);

        if (original == null)
            return NotFound();

        Cliente? cliente = repositorioCliente.SelecionarPorId(viewModel.ClienteId);

        if (cliente == null)
            ModelState.AddModelError(nameof(viewModel.ClienteId), "Selecione um cliente.");

        // Retira a saída antiga do cálculo para que a nova quantidade seja validada
        // contra o estoque que realmente ficará disponível após a edição.
        RemoverRequisicaoDosProdutos(original);

        List<ProdutoPrescrito> produtosPrescritos = CriarProdutosSelecionados(
            viewModel.ProdutosPrescritos,
            out _);

        if (cliente != null)
        {
            // Não usamos o construtor que registra automaticamente a requisição.
            // Primeiro validamos. Só depois registramos a instância original.
            RequisicaoSaida atualizada = new RequisicaoSaida
            {
                Id = original.Id,
                Cliente = cliente,
                ProdutosPrescritos = produtosPrescritos,
                Data = original.Data
            };

            foreach (string erro in atualizada.Validar())
                ModelState.AddModelError(string.Empty, erro);

            if (ModelState.IsValid)
            {
                original.Cliente = cliente;
                original.ProdutosPrescritos = produtosPrescritos;

                foreach (ProdutoPrescrito prescrito in original.ProdutosPrescritos)
                    prescrito.Produto.RegistrarRequisicaoSaida(original);

                if (repositorio.Editar(viewModel.Id, original))
                    return RedirectToAction(nameof(Listar));

                RemoverRequisicaoDosProdutos(original);
                return NotFound();
            }
        }

        // Se a edição falhou, recoloca a requisição original nos produtos.
        foreach (ProdutoPrescrito prescrito in original.ProdutosPrescritos)
            prescrito.Produto.RegistrarRequisicaoSaida(original);

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

        RemoverRequisicaoDosProdutos(requisicao);

        if (!repositorio.Excluir(id))
        {
            foreach (ProdutoPrescrito prescrito in requisicao.ProdutosPrescritos)
                prescrito.Produto.RegistrarRequisicaoSaida(requisicao);

            return NotFound();
        }

        return RedirectToAction(nameof(Listar));
    }

    private bool CorrespondePesquisa(RequisicaoSaida requisicao, string pesquisa)
    {
        if (requisicao.Id.ToString().Contains(pesquisa, StringComparison.OrdinalIgnoreCase))
            return true;

        if (requisicao.Data.ToString("dd/MM/yyyy").Contains(pesquisa, StringComparison.OrdinalIgnoreCase))
            return true;

        if ((requisicao.Cliente?.Nome ?? string.Empty).Contains(pesquisa, StringComparison.OrdinalIgnoreCase))
            return true;

        foreach (ProdutoPrescrito prescrito in requisicao.ProdutosPrescritos)
        {
            string nomeProduto = prescrito.Produto?.Nome ?? string.Empty;

            if (nomeProduto.Contains(pesquisa, StringComparison.OrdinalIgnoreCase))
                return true;

            if (prescrito.Quantidade.ToString().Contains(pesquisa, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private List<ProdutoPrescrito> CriarProdutosSelecionados(
        List<ProdutoPrescritoRequisicaoSaidaViewModel>? produtosModel,
        out List<Produto> produtosRegistrados)
    {
        produtosRegistrados = new List<Produto>();
        List<ProdutoPrescrito> produtosPrescritos = new List<ProdutoPrescrito>();

        if (produtosModel == null)
            return produtosPrescritos;

        foreach (ProdutoPrescritoRequisicaoSaidaViewModel produtoModel in produtosModel)
        {
            if (!produtoModel.Selecionado)
                continue;

            Produto? produto = repositorioProduto.SelecionarPorId(produtoModel.ProdutoId);

            if (produto == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Um dos produtos selecionados não existe mais.");
                continue;
            }

            produtosPrescritos.Add(
                new ProdutoPrescrito(produto, produtoModel.Quantidade));

            produtosRegistrados.Add(produto);
        }

        return produtosPrescritos;
    }

    private void RemoverRequisicaoDosProdutos(RequisicaoSaida requisicao)
    {
        foreach (ProdutoPrescrito prescrito in requisicao.ProdutosPrescritos)
        {
            if (prescrito.Produto != null)
                prescrito.Produto.RemoverRequisicaoSaida(requisicao);
        }
    }

    private List<ClienteRequisicaoSaidaViewModel> ObterClientes()
    {
        List<ClienteRequisicaoSaidaViewModel> viewModels =
            new List<ClienteRequisicaoSaidaViewModel>();

        foreach (Cliente cliente in repositorioCliente.SelecionarTodos())
        {
            viewModels.Add(
                new ClienteRequisicaoSaidaViewModel(cliente.Id, cliente.Nome));
        }

        return viewModels;
    }

    private List<ProdutoPrescritoRequisicaoSaidaViewModel> ObterProdutos(
        List<ProdutoPrescritoRequisicaoSaidaViewModel>? valoresEnviados = null)
    {
        Dictionary<int, ProdutoPrescritoRequisicaoSaidaViewModel> valoresPorProduto =
            new Dictionary<int, ProdutoPrescritoRequisicaoSaidaViewModel>();

        if (valoresEnviados != null)
        {
            foreach (ProdutoPrescritoRequisicaoSaidaViewModel valor in valoresEnviados)
                valoresPorProduto[valor.ProdutoId] = valor;
        }

        List<ProdutoPrescritoRequisicaoSaidaViewModel> viewModels =
            new List<ProdutoPrescritoRequisicaoSaidaViewModel>();

        foreach (Produto produto in repositorioProduto.SelecionarTodos())
        {
            valoresPorProduto.TryGetValue(
                produto.Id,
                out ProdutoPrescritoRequisicaoSaidaViewModel? valor);

            viewModels.Add(
                new ProdutoPrescritoRequisicaoSaidaViewModel(
                    produto.Id,
                    produto.Nome,
                    produto.QuantidadeEmEstoque,
                    valor?.Selecionado ?? false,
                    valor?.Quantidade ?? 0));
        }

        return viewModels;
    }

    private List<ProdutoPrescritoRequisicaoSaidaViewModel> ObterProdutosParaEdicao(
        RequisicaoSaida requisicao)
    {
        Dictionary<int, int> quantidades =
            new Dictionary<int, int>();

        foreach (ProdutoPrescrito prescrito in requisicao.ProdutosPrescritos)
        {
            if (prescrito.Produto != null)
                quantidades[prescrito.Produto.Id] = prescrito.Quantidade;
        }

        List<ProdutoPrescritoRequisicaoSaidaViewModel> viewModels =
            new List<ProdutoPrescritoRequisicaoSaidaViewModel>();

        foreach (Produto produto in repositorioProduto.SelecionarTodos())
        {
            bool selecionado = quantidades.ContainsKey(produto.Id);
            int quantidade = quantidades.TryGetValue(produto.Id, out int valor) ? valor : 0;

            // Durante a edição, a quantidade da própria saída ainda está descontada
            // do estoque. Por isso ela é devolvida apenas para a exibição.
            int estoqueDisponivel = produto.QuantidadeEmEstoque + quantidade;

            viewModels.Add(
                new ProdutoPrescritoRequisicaoSaidaViewModel(
                    produto.Id,
                    produto.Nome,
                    estoqueDisponivel,
                    selecionado,
                    quantidade));
        }

        return viewModels;
    }
}
