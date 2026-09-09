using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.ModuloFornecedores;

namespace ControleDeEstoque.WebApp.ModuloProdutos;

public class TelaProduto : TelaBase<Produto>, ITelaOpcoes, ITelaCrud
{
    private readonly RepositorioFornecedorEmArquivo repositorioFornecedor;

    public TelaProduto(
        RepositorioProdutoEmArquivo repositorioProduto,
        RepositorioFornecedorEmArquivo repositorioFornecedor
    ) : base("Produto", repositorioProduto)
    {
        this.repositorioFornecedor = repositorioFornecedor;
    }

    public override void VisualizarTodos(bool deveExibirCabecalho)
    {
        if (deveExibirCabecalho)
        {
            Console.Clear();
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Visualização de Produtos");
            Console.WriteLine("---------------------------------");
        }

        Console.WriteLine(
            "{0, -7} | {1, -20} | {2, -20} | {3, -20} | {4, -10}",
            "Id", "Nome", "Fornecedor", "Descrição", "Estoque"
        );

        List<Produto> registros = repositorio.SelecionarTodos();

        foreach (Produto m in registros)
        {
            Console.WriteLine(
                "{0, -7} | {1, -20} | {2, -20} | {3, -20} | {4, -10}",
                m.Id, m.Nome, m.Fornecedor.Nome, m.Descricao, m.QuantidadeEmEstoque
            );
        }

        if (deveExibirCabecalho)
        {
            Console.WriteLine("---------------------------------");
            Console.Write("Pressione ENTER para continuar...");
            Console.ReadLine();
        }
    }

    protected override Produto ObterDadosCadastrais()
    {

        Console.Write("Digite o nome do produto: ");
        string nome = Console.ReadLine() ?? string.Empty;

        Console.Write("Digite a descrição do produto: ");
        string descricao = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("---------------------------------");

        VisualizarFornecedores();

        Console.WriteLine("---------------------------------");

        Console.Write("Digite o ID do fornecedor que deseja selecionar: ");
        int idFornecedor = Convert.ToInt32(Console.ReadLine());

        Fornecedor fornecedor = repositorioFornecedor.SelecionarPorId(idFornecedor)!;

        return new Produto(nome, descricao, fornecedor);

    }

    private void VisualizarFornecedores()
    {
        Console.WriteLine(
            "{0, -7} | {1, -30} | {2, -15} | {3, -17}",
            "Id", "Nome", "Telefone", "CNPJ"
        );

        List<Fornecedor> registros = repositorioFornecedor.SelecionarTodos();

        foreach (Fornecedor f in registros)
        {
            Console.WriteLine(
                "{0, -7} | {1, -30} | {2, -15} | {3, -17}",
                f.Id, f.Nome, f.Telefone, f.Cnpj
            );
        }
    }

    protected override bool ExisteRegistroComInformacoesExclusivas(Produto entidade, int? idIgnorado = null)
    {
        List<Produto> registros = repositorio.SelecionarTodos();

        foreach (Produto m in registros)
        {
            if (m.Id != idIgnorado && m.Nome.Equals(entidade.Nome, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("---------------------------------");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Já existe um produto cadastrado com este nome.");
                Console.ResetColor();
                Console.WriteLine("---------------------------------");
                return true;
            }
        }

        return false;
    }
}
