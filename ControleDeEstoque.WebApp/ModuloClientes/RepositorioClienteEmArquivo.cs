using ControleDeEstoque.WebApp.Compartilhado.Arquivos;

namespace ControleDeEstoque.WebApp.ModuloClientes;

public class RepositorioClienteEmArquivo : RepositorioBaseEmArquivo<Cliente>
{
    public RepositorioClienteEmArquivo(ContextoJson contexto) : base(contexto)
    {
    }

    protected override List<Cliente> ObterRegistros()
    {
        return contexto.Cliente;
    }
}
