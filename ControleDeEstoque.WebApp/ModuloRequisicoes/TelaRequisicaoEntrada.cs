using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.ModuloFuncionario;
using ControleDeEstoque.WebApp.ModuloProdutos;

namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public class TelaRequisicaoEntrada : TelaBase<RequisicaoEntrada>, ITelaOpcoes, ITelaCrud
{
    private readonly RepositorioProdutoEmArquivo repositorioProduto;
    private readonly RepositorioFuncionarioEmArquivo repositorioFuncionario;

    public TelaRequisicaoEntrada(
        RepositorioRequisicaoEntradaEmArquivo repositorioRequisicao,
        RepositorioProdutoEmArquivo repositorioProduto,
        RepositorioFuncionarioEmArquivo repositorioFuncionario
    ) : base("Requisição de Entrada", repositorioRequisicao)
    {
        this.repositorioProduto = repositorioProduto;
        this.repositorioFuncionario = repositorioFuncionario;
    }

    protected override bool ExistemDependenciasAtivasDoRegistro(int idRegistro)
    {
        return false;
    }

    public override void VisualizarTodos(bool deveExibirCabecalho)
    {
        if (deveExibirCabecalho)
        {
            Console.Clear();
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Visualização de Requisições de Entrada");
            Console.WriteLine("---------------------------------");
        }

        Console.WriteLine(
            "{0, -7} | {1, -20} | {2, -10} | {3, -15}",
            "Id", "Produto", "Qtd", "Data"
        );

        List<RequisicaoEntrada> registros = repositorio.SelecionarTodos();

        foreach (RequisicaoEntrada r in registros)
        {
            Console.WriteLine(
                "{0, -7} | {1, -20} | {2, -10} | {3, -15}",
                r.Id, r.Produto.Nome, r.Quantidade, r.Data.ToShortDateString()
            );
        }

        if (deveExibirCabecalho)
        {
            Console.WriteLine("---------------------------------");
            Console.Write("Digite ENTER para continuar...");
            Console.ReadLine();
        }
    }

    protected override RequisicaoEntrada ObterDadosCadastrais()
    {
        VisualizarProdutos();

        Console.WriteLine("---------------------------------");

        Console.Write("Digite o ID do produto que deseja requisitar: ");
        int idProduto = Convert.ToInt32(Console.ReadLine());
        Produto produto = repositorioProduto.SelecionarPorId(idProduto)!;

        Console.WriteLine("---------------------------------");

        Console.Write("Digite a quantidade do produto que deseja requisitar: ");
        int quantidade = Convert.ToInt32(Console.ReadLine());

        VisualizarFuncionarios();

        Console.Write("Digite o ID do funcionário requisitante: ");
        int idFuncionario = Convert.ToInt32(Console.ReadLine());
        Funcionario funcionario = repositorioFuncionario.SelecionarPorId(idFuncionario)!;

        return new RequisicaoEntrada(produto, quantidade, funcionario);
    }

    private void VisualizarProdutos()
    {
        Console.WriteLine(
            "{0, -7} | {1, -20} | {2, -20} | {3, -20}",
            "Id", "Nome", "Fornecedor", "Descrição"
        );

        List<Produto> registros = repositorioProduto.SelecionarTodos();

        foreach (Produto m in registros)
        {
            Console.WriteLine(
                "{0, -7} | {1, -20} | {2, -20} | {3, -20}",
                m.Id, m.Nome, m.Fornecedor.Nome, m.Descricao
            );
        }
    }

    private void VisualizarFuncionarios()
    {
        Console.WriteLine(
            "{0, -7} | {1, -30} | {2, -15} | {3, -14}",
            "Id", "Nome", "Telefone", "CPF"
        );

        List<Funcionario> registros = repositorioFuncionario.SelecionarTodos();

        foreach (Funcionario f in registros)
        {
            Console.WriteLine(
                "{0, -7} | {1, -30} | {2, -15} | {3, -14}",
                f.Id, f.Nome, f.Telefone, f.Cpf
            );
        }
    }
}
