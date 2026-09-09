using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.ModuloFuncionario;

namespace ControleDeEstoque.WebApp.ModuloFuncionario;

public class TelaFuncionario : TelaBase<Funcionario>, ITelaOpcoes, ITelaCrud
{
    public TelaFuncionario(RepositorioFuncionarioEmArquivo repositorio) : base("Funcionario", repositorio)
    {
    }
    public override void VisualizarTodos(bool deveExibirCabecalho)
    {
        if (deveExibirCabecalho)
        {
            Console.Clear();
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Visualização de Funcionários");
            Console.WriteLine("---------------------------------");
        }

        Console.WriteLine(
            "{0,-5} | {1,-30} | {2,-15} | {3,-11}",
            "Id", "Nome", "Telefone", "CPF"
        );

        List<Funcionario> registros = repositorio.SelecionarTodos();

        foreach (Funcionario f in registros)
        {
            Console.WriteLine(
               "{0,-5} | {1,-30} | {2,-15} | {3,-11}",
               f.Id,
               f.Nome,
               f.Telefone,
               f.Cpf
           );
        }

        if (deveExibirCabecalho)
        {
            Console.WriteLine("---------------------------------");
            Console.Write("Pressione ENTER para continuar...");
            Console.ReadLine();
        }
    }
    protected override Funcionario ObterDadosCadastrais()
    {

        Console.Write("Digite o nome do funcionário: ");
        string nome = Console.ReadLine() ?? string.Empty;

        Console.Write("Digite o telefone do funcionário: ");
        string telefone = Console.ReadLine() ?? string.Empty;

        Console.Write("Informe o CPF do funcionário: ");
        string Cpf = Console.ReadLine() ?? string.Empty;

        return new Funcionario(nome, telefone, Cpf);

    }

    protected override bool ExisteRegistroComInformacoesExclusivas(Funcionario entidade, int? idIgnorado = null)
    {
        List<Funcionario> registros = repositorio.SelecionarTodos();

        foreach (Funcionario funcionario in registros)
        {
            if (funcionario.Id != idIgnorado && funcionario.Cpf == entidade.Cpf)
            {
                Console.WriteLine("---------------------------------");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Já existe um funcionário cadastrado com este CPF.");
                Console.ResetColor();
                Console.WriteLine("---------------------------------");

                return true;
            }
        }

        return false;
    }
}
