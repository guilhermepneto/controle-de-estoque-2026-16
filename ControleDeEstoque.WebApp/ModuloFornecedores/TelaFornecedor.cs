using ControleDeEstoque.WebApp.Compartilhado;

namespace ControleDeEstoque.WebApp.ModuloFornecedores;

public class TelaFornecedor : TelaBase<Fornecedor>, ITelaOpcoes, ITelaCrud
{
    public TelaFornecedor(RepositorioFornecedorEmArquivo repositorio) : base("Fornecedor", repositorio)
    {
    }

    public override void VisualizarTodos(bool deveExibirCabecalho)
    {
        if (deveExibirCabecalho)
        {
            Console.Clear();
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Visualização de Fornecedores");
            Console.WriteLine("---------------------------------");
        }

        Console.WriteLine(
            "{0, -7} | {1, -30} | {2, -15} | {3, -17}",
            "Id", "Nome", "Telefone", "CNPJ"
        );

        List<Fornecedor> registros = repositorio.SelecionarTodos();

        foreach (Fornecedor f in registros)
        {
            Console.WriteLine(
                "{0, -7} | {1, -30} | {2, -15} | {3, -17}",
                f.Id, f.Nome, f.Telefone, f.Cnpj
            );
        }

        if (deveExibirCabecalho)
        {
            Console.WriteLine("---------------------------------");
            Console.Write("Pressione ENTER para continuar...");
            Console.ReadLine();
        }
    }

    protected override Fornecedor ObterDadosCadastrais()
    {

        Console.Write("Digite o nome do fornecedor: ");
        string nome = Console.ReadLine() ?? string.Empty;

        Console.Write("Digite o telefone do fornecedor: ");
        string telefone = Console.ReadLine() ?? string.Empty;

        Console.Write("Digite o CNPJ do fornecedor: ");
        string cnpj = Console.ReadLine() ?? string.Empty;

        return new Fornecedor(nome, telefone, cnpj);

    }

    protected override bool ExisteRegistroComInformacoesExclusivas(Fornecedor entidade, int? idIgnorado = null)
    {
        List<Fornecedor> registros = repositorio.SelecionarTodos();

        foreach (Fornecedor f in registros)
        {
            if (f.Id != idIgnorado && f.Cnpj == entidade.Cnpj)
            {
                Console.WriteLine("---------------------------------");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Já existe um fornecedor cadastrado com o CNPJ informado.");
                Console.ResetColor();
                Console.WriteLine("---------------------------------");
                return true;
            }
        }

        return false;
    }
}
