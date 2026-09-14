using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.ModuloFornecedores;
using ControleDeEstoque.WebApp.ModuloRequisicoes;

namespace ControleDeEstoque.WebApp.ModuloProdutos;

public enum CategoriaProduto
{
    Geral,
    Tinta,
    Toner
}

public class Produto : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string MarcaEquipamento { get; set; } = string.Empty;
    public string MarcaItem { get; set; } = string.Empty;
    public Fornecedor Fornecedor { get; set; } = null!;
    public CategoriaProduto Categoria { get; set; } = CategoriaProduto.Geral;
    public List<RequisicaoEntrada> Requisicoes { get; set; } = [];
    public List<RequisicaoSaida> RequisicoesSaida { get; set; } = [];


    public Produto() { }

    public Produto(string nome, string descricao, Fornecedor fornecedor, CategoriaProduto categoria = CategoriaProduto.Geral, string marcaEquipamento = "", string marcaItem = "") : this()
    {
        Nome = nome;
        Descricao = descricao;
        Fornecedor = fornecedor;
        Categoria = categoria;
        MarcaEquipamento = marcaEquipamento;
        MarcaItem = marcaItem;
    }

    public int QuantidadeEmEstoque
    {
        get
        {
            int total = 0;

            foreach (RequisicaoEntrada req in Requisicoes)
                total += req.Quantidade;

            foreach (RequisicaoSaida req in RequisicoesSaida)
                total -= req.ObterQuantidade(this);

            return total;
        }
    }

    public void RegistrarRequisicao(RequisicaoEntrada requisicao)
    {
        Requisicoes.Add(requisicao);
    }

    public void RegistrarRequisicaoSaida(RequisicaoSaida requisicaoSaida)
    {
        RequisicoesSaida.Add(requisicaoSaida);
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Nome) || Nome.Length < 2 || Nome.Length > 100)
            erros.Add("O campo \"Nome\" deve conter entre 2 e 100 caracteres.");

        if (string.IsNullOrWhiteSpace(Descricao) || Descricao.Length < 5 || Descricao.Length > 255)
            erros.Add("O campo \"Descrição\" deve conter entre 5 e 255 caracteres.");

        if (Fornecedor == null)
            erros.Add("O campo \"Fornecedor\" deve ser preenchido.");

        if (string.IsNullOrWhiteSpace(MarcaEquipamento))
            erros.Add("O campo \"Marca do equipamento\" deve ser preenchido.");

        if (string.IsNullOrWhiteSpace(MarcaItem))
            erros.Add("O campo \"Marca do item\" deve ser preenchido.");

        return erros;
    }

    public override void Atualizar(EntidadeBase entidadeAtualizada)
    {
        Produto produtoAtualizado = (Produto)entidadeAtualizada;

        Nome = produtoAtualizado.Nome;
        Descricao = produtoAtualizado.Descricao;
        Fornecedor = produtoAtualizado.Fornecedor;
        Categoria = produtoAtualizado.Categoria;
        MarcaEquipamento = produtoAtualizado.MarcaEquipamento;
        MarcaItem = produtoAtualizado.MarcaItem;
    }

}
