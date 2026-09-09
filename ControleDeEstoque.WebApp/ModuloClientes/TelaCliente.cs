using ControleDeEstoque.WebApp.Compartilhado;

namespace ControleDeEstoque.WebApp.ModuloClientes;

public class TelaCliente : TelaBase<Cliente>, ITelaOpcoes, ITelaCrud
{
    public TelaCliente(RepositorioClienteEmArquivo repositorio) : base("Cliente", repositorio)
    {
    }

    public override void VisualizarTodos(bool deveExibirCabecalho)
    {
        if (deveExibirCabecalho)
        {
            Console.Clear();
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Visualização de Clientes");
            Console.WriteLine("---------------------------------");
        }

        Console.WriteLine(
            "{0,-5} | {1,-30} | {2,-15} | {3,-15} | {4,-11}",
            "Id", "Nome", "Telefone", "E-mail", "CPF"
        );

        List<Cliente> registros = repositorio.SelecionarTodos();

        foreach (Cliente p in registros)
        {
            Console.WriteLine(
               "{0,-5} | {1,-30} | {2,-15} | {3,-15} | {4,-11}",
               p.Id,
               p.Nome,
               p.Telefone,
               p.Email,
               p.Cpf
           );
        }

        if (deveExibirCabecalho)
        {
            Console.WriteLine("---------------------------------");
            Console.Write("Pressione ENTER para continuar...");
            Console.ReadLine();
        }
    }

    protected override Cliente ObterDadosCadastrais()
    {

        Console.Write("Digite o nome do cliente: ");
        string nome = Console.ReadLine() ?? string.Empty;

        Console.Write("Digite o telefone do cliente: ");
        string telefone = Console.ReadLine() ?? string.Empty;

        Console.Write("Digite o e-mail do cliente: ");
        string email = Console.ReadLine() ?? string.Empty;

        Console.Write("Informe o CPF do cliente: ");
        string Cpf = Console.ReadLine() ?? string.Empty;

        return new Cliente(nome, telefone, email, Cpf);

    }

    protected override bool ExisteRegistroComInformacoesExclusivas(Cliente entidade, int? idIgnorado = null)
    {
        List<Cliente> registros = repositorio.SelecionarTodos();

        foreach (Cliente cliente in registros)
        {
            if (cliente.Id != idIgnorado && cliente.Email == entidade.Email)
            {
                Console.WriteLine("---------------------------------");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Já existe um cliente cadastrado com este E-mail.");
                Console.ResetColor();
                Console.WriteLine("---------------------------------");

                return true;
            }
        }

        return false;
    }
}
