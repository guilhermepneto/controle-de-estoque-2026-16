using ControleDeEstoque.WebApp.Compartilhado.Arquivos;
using ControleDeEstoque.WebApp.ModuloFornecedores;
using ControleDeEstoque.WebApp.ModuloFuncionario;
using ControleDeEstoque.WebApp.ModuloProdutos;
using ControleDeEstoque.WebApp.ModuloClientes;
using ControleDeEstoque.WebApp.ModuloRequisicoes;

namespace ControleDeEstoque.WebApp.Compartilhado;

public class TelaPrincipal
{
    private readonly TelaFornecedor telaFornecedor;
    private readonly TelaProduto telaProduto;
    private readonly TelaRequisicaoEntrada telaRequisicaoEntrada;
    private readonly TelaRequisicaoSaida telaRequisicaoSaida;
    private readonly TelaCliente telaCliente;
    private readonly TelaFuncionario telaFuncionario;

    public TelaPrincipal(ContextoJson contexto)
    {
        RepositorioFornecedorEmArquivo repositorioFornecedor = new RepositorioFornecedorEmArquivo(contexto);
        RepositorioProdutoEmArquivo repositorioProduto = new RepositorioProdutoEmArquivo(contexto);
        RepositorioRequisicaoEntradaEmArquivo repositorioRequisicao = new RepositorioRequisicaoEntradaEmArquivo(contexto);
        RepositorioRequisicaoSaidaEmArquivo repositorioRequisicaoSaida = new RepositorioRequisicaoSaidaEmArquivo(contexto);
        RepositorioClienteEmArquivo repositorioCliente = new RepositorioClienteEmArquivo(contexto);
        RepositorioFuncionarioEmArquivo repositorioFuncionario = new RepositorioFuncionarioEmArquivo(contexto);

        telaFornecedor = new TelaFornecedor(repositorioFornecedor);
        telaProduto = new TelaProduto(repositorioProduto, repositorioFornecedor);
        telaRequisicaoEntrada = new TelaRequisicaoEntrada(repositorioRequisicao, repositorioProduto, repositorioFuncionario);
        telaRequisicaoSaida = new TelaRequisicaoSaida(repositorioRequisicaoSaida, repositorioCliente, repositorioProduto);
        telaCliente = new TelaCliente(repositorioCliente);
        telaFuncionario = new TelaFuncionario(repositorioFuncionario);
    }

    public ITelaOpcoes? ObterOpcaoMenuPrincipal()
    {
        Console.Clear();
        Console.WriteLine("---------------------------------");
        Console.WriteLine("Controle de Estoque");
        Console.WriteLine("---------------------------------");
        Console.WriteLine("1 - Gestão de Fornecedores");
        Console.WriteLine("2 - Gestão de Produtos");
        Console.WriteLine("3 - Gestão de Clientes");
        Console.WriteLine("4 - Gestão de Funcionários");
        Console.WriteLine("5 - Gestão de Requisições de Entrada");
        Console.WriteLine("6 - Gestão de Requisições de Saída");
        Console.WriteLine("S - Sair");
        Console.WriteLine("---------------------------------");
        Console.Write("> ");

        string? opcaoMenuPrincipal = Console.ReadLine()?.ToUpper();

        if (opcaoMenuPrincipal == "1")
            return telaFornecedor;

        if (opcaoMenuPrincipal == "2")
            return telaProduto;

        if (opcaoMenuPrincipal == "3")
            return telaCliente;

        if (opcaoMenuPrincipal == "4")
            return telaFuncionario;

        if (opcaoMenuPrincipal == "5")
            return telaRequisicaoEntrada;

        if (opcaoMenuPrincipal == "6")
            return telaRequisicaoSaida;

        return null;
    }
}
