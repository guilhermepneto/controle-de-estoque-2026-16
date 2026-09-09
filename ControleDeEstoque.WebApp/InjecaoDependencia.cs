using ControleDeEstoque.WebApp.Compartilhado.Arquivos;
using ControleDeEstoque.WebApp.ModuloFornecedores;
using ControleDeEstoque.WebApp.ModuloFuncionario;
using ControleDeEstoque.WebApp.ModuloProdutos;
using ControleDeEstoque.WebApp.ModuloClientes;
using ControleDeEstoque.WebApp.ModuloRequisicoes;

namespace ControleDeEstoque.WebApp;

public static class InjecaoDependencia
{
    public static void AddInfraestruturaEmJson(this IServiceCollection services)
    {
        services.AddScoped(_ =>
{
    ContextoJson contexto = new ContextoJson();
    contexto.Carregar();

    return contexto;
});

        services.AddScoped<RepositorioProdutoEmArquivo>();
        services.AddScoped<RepositorioFornecedorEmArquivo>();
        services.AddScoped<RepositorioFuncionarioEmArquivo>();
        services.AddScoped<RepositorioClienteEmArquivo>();
        services.AddScoped<RepositorioRequisicaoEntradaEmArquivo>();
        services.AddScoped<RepositorioRequisicaoSaidaEmArquivo>();
    }
}
