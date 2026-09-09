using System.Text.Json;
using System.Text.Json.Serialization;
using ControleDeEstoque.WebApp.ModuloFornecedores;
using ControleDeEstoque.WebApp.ModuloFuncionario;
using ControleDeEstoque.WebApp.ModuloProdutos;
using ControleDeEstoque.WebApp.ModuloClientes;
using ControleDeEstoque.WebApp.ModuloRequisicoes;

namespace ControleDeEstoque.WebApp.Compartilhado.Arquivos;

public class ContextoJson
{
    private readonly string caminhoArquivoDados;

    public List<Fornecedor> Fornecedores { get; set; } = [];
    public List<Produto> Produtos { get; set; } = [];
    public List<Cliente> Cliente { get; set; } = [];
    public List<Funcionario> Funcionario { get; set; } = [];
    public List<RequisicaoEntrada> RequisicoesEntrada { get; set; } = [];
    public List<RequisicaoSaida> RequisicaoSaida { get; set; } = [];


    public ContextoJson()
    {
        string caminhoAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string caminhoDiretorioAplicativo = Path.Join(caminhoAppData, "ControleDeEstoque-Backend");

        Directory.CreateDirectory(caminhoDiretorioAplicativo);

        caminhoArquivoDados = Path.Join(caminhoDiretorioAplicativo, "dados.json");
    }
    public void Salvar()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        options.ReferenceHandler = ReferenceHandler.Preserve;

        string jsonString = JsonSerializer.Serialize(this, options);

        File.WriteAllText(caminhoArquivoDados, jsonString);
    }

    public void Carregar()
    {
        if (!File.Exists(caminhoArquivoDados))
            return;

        string jsonString = File.ReadAllText(caminhoArquivoDados);

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        options.ReferenceHandler = ReferenceHandler.Preserve;

        ContextoJson? contextoSalvo =
            JsonSerializer.Deserialize<ContextoJson>(jsonString, options);

        if (contextoSalvo == null)
            return;

        Fornecedores = contextoSalvo.Fornecedores;
        Produtos = contextoSalvo.Produtos;
        Cliente = contextoSalvo.Cliente;
        Funcionario = contextoSalvo.Funcionario;
        RequisicoesEntrada = contextoSalvo.RequisicoesEntrada;
        RequisicaoSaida = contextoSalvo.RequisicaoSaida;
    }
}
