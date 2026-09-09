using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.Compartilhado.Arquivos;

namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public class RepositorioRequisicaoEntradaEmArquivo : RepositorioBaseEmArquivo<RequisicaoEntrada>
{
    public RepositorioRequisicaoEntradaEmArquivo(ContextoJson contexto) : base(contexto)
    {
    }

    protected override List<RequisicaoEntrada> ObterRegistros()
    {
        return contexto.RequisicoesEntrada;
    }
}
