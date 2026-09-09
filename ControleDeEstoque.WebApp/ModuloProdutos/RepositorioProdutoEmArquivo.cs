using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.Compartilhado.Arquivos;

namespace ControleDeEstoque.WebApp.ModuloProdutos;

public class RepositorioProdutoEmArquivo : RepositorioBaseEmArquivo<Produto>
{
    public RepositorioProdutoEmArquivo(ContextoJson contexto) : base(contexto)
    {
    }

    protected override List<Produto> ObterRegistros()
    {
        return contexto.Produtos;
    }
}
