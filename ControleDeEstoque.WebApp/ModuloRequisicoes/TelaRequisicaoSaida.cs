using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.ModuloProdutos;
using ControleDeEstoque.WebApp.ModuloClientes;

namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public class TelaRequisicaoSaida : TelaBase<RequisicaoSaida>, ITelaOpcoes, ITelaCrud
{
    private readonly RepositorioClienteEmArquivo repositorioCliente;
    private readonly RepositorioProdutoEmArquivo repositorioProduto;

    public TelaRequisicaoSaida(
        RepositorioRequisicaoSaidaEmArquivo repositorioRequisicao,
        RepositorioClienteEmArquivo repositorioCliente,
        RepositorioProdutoEmArquivo repositorioProduto
    ) : base("Requisição de Saída", repositorioRequisicao)
    {
        this.repositorioCliente = repositorioCliente;
        this.repositorioProduto = repositorioProduto;
    }

    public override void VisualizarTodos(bool deveExibirCabecalho)
    {
        if (deveExibirCabecalho)
        {
            Console.Clear();
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Visualização de Requisições de Saída");
            Console.WriteLine("---------------------------------");
        }

        List<RequisicaoSaida> registros = repositorio.SelecionarTodos();

        foreach (RequisicaoSaida r in registros)
        {
            Console.WriteLine("Id: {0} | Cliente: {1} | Data: {2}",
                r.Id, r.Cliente.Nome, r.Data.ToShortDateString());

            Console.WriteLine(
                "{0, -7} | {1, -20} | {2, -10}",
                "Id", "Produto", "Qtd"
            );

            foreach (ProdutoPrescrito mp in r.ProdutosPrescritos)
            {
                Console.WriteLine(
                    "{0, -7} | {1, -20} | {2, -10}",
                    mp.Produto.Id, mp.Produto.Nome, mp.Quantidade
                );
            }

            Console.WriteLine("---------------------------------");
        }

        if (deveExibirCabecalho)
        {
            Console.Write("Digite ENTER para continuar...");
            Console.ReadLine();
        }
    }

    protected override RequisicaoSaida ObterDadosCadastrais()
    {
        VisualizarClientes();

        Console.WriteLine("---------------------------------");

        Console.Write("Digite o ID do cliente: ");
        int idCliente = Convert.ToInt32(Console.ReadLine());

        Cliente cliente = repositorioCliente.SelecionarPorId(idCliente)!;

        List<ProdutoPrescrito> produtosPrescritos = [];

        while (true)
        {
            VisualizarProdutos();

            Console.WriteLine("---------------------------------");

            Console.Write("Digite o ID do produto (0 para finalizar): ");
            int idProduto = Convert.ToInt32(Console.ReadLine());

            if (idProduto == 0)
                break;

            Produto produto = repositorioProduto.SelecionarPorId(idProduto)!;

            Console.Write("Digite a quantidade: ");
            int quantidade = Convert.ToInt32(Console.ReadLine());

            produtosPrescritos.Add(new ProdutoPrescrito(produto, quantidade));
        }

        return new RequisicaoSaida(cliente, produtosPrescritos);
    }

    private void VisualizarClientes()
    {
        Console.WriteLine(
            "{0, -7} | {1, -30} | {2, -15} | {3, -17} | {4, -14}",
            "Id", "Nome", "Telefone", "E-mail", "CPF"
        );

        List<Cliente> registros = repositorioCliente.SelecionarTodos();

        foreach (Cliente p in registros)
        {
            Console.WriteLine(
                "{0, -7} | {1, -30} | {2, -15} | {3, -17} | {4, -14}",
                p.Id, p.Nome, p.Telefone, p.Email, p.Cpf
            );
        }
    }

    private void VisualizarProdutos()
    {
        Console.WriteLine(
            "{0, -7} | {1, -20} | {2, -20} | {3, -20} | {4, -10}",
            "Id", "Nome", "Fornecedor", "Descrição", "Estoque"
        );

        List<Produto> registros = repositorioProduto.SelecionarTodos();

        foreach (Produto m in registros)
        {
            Console.WriteLine(
                "{0, -7} | {1, -20} | {2, -20} | {3, -20} | {4, -10}",
                m.Id, m.Nome, m.Fornecedor.Nome, m.Descricao, m.QuantidadeEmEstoque
            );
        }
    }

    protected override bool ExistemDependenciasAtivasDoRegistro(int idRegistro)
    {
        return false;
    }
}
