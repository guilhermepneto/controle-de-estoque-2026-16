using ControleDeEstoque.WebApp.Compartilhado.Arquivos;

namespace ControleDeEstoque.WebApp.ModuloFornecedores;

public class RepositorioFornecedorEmArquivo : RepositorioBaseEmArquivo<Fornecedor>
{
    public RepositorioFornecedorEmArquivo(ContextoJson contexto) : base(contexto)
    {
    }

    protected override List<Fornecedor> ObterRegistros()
    {
        return contexto.Fornecedores;
    }
}
