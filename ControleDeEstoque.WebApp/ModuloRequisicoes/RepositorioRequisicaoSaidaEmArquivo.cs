using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.Compartilhado.Arquivos;

namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public class RepositorioRequisicaoSaidaEmArquivo : RepositorioBaseEmArquivo<RequisicaoSaida>
{
    public RepositorioRequisicaoSaidaEmArquivo(ContextoJson contexto) : base(contexto)
    {
    }

    protected override List<RequisicaoSaida> ObterRegistros()
    {
        return contexto.RequisicaoSaida;
    }
}
