using ControleDeEstoque.WebApp.Compartilhado.Arquivos;
using ControleDeEstoque.WebApp.ModuloFuncionario;


namespace ControleDeEstoque.WebApp.ModuloFuncionario;

public class RepositorioFuncionarioEmArquivo : RepositorioBaseEmArquivo<Funcionario>
{
    public RepositorioFuncionarioEmArquivo(ContextoJson contexto) : base(contexto)
    {
    }

    protected override List<Funcionario> ObterRegistros()
    {
        return contexto.Funcionario;
    }
}
