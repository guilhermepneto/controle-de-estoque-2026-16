using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.ModuloFuncionario;
using ControleDeEstoque.WebApp.ModuloProdutos;

namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public enum TipoEntrada
{
    NotaFiscal,
    Devolucao
}

public class RequisicaoEntrada : EntidadeBase
{
    public Produto Produto { get; set; } = null!;
    public Funcionario Funcionario { get; set; } = null!;
    public int Quantidade { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
    public TipoEntrada Tipo { get; set; } = TipoEntrada.NotaFiscal;
    public string? NumeroNotaFiscal { get; set; }

    public RequisicaoEntrada() { }

    public RequisicaoEntrada(
        Produto produto,
        int quantidade,
        Funcionario funcionario,
        TipoEntrada tipo = TipoEntrada.NotaFiscal,
        string? numeroNotaFiscal = null) : this()
    {
        Produto = produto;
        Quantidade = quantidade;
        Funcionario = funcionario;
        Tipo = tipo;
        NumeroNotaFiscal = numeroNotaFiscal;

        produto.RegistrarRequisicao(this);
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (Produto == null)
            erros.Add("O campo \"Produto\" deve ser preenchido.");

        if (Funcionario == null)
            erros.Add("O campo \"Funcionário\" deve ser preenchido.");

        if (Quantidade <= 0)
            erros.Add("A \"Quantidade\" deve ser maior que zero.");

        if (Tipo == TipoEntrada.NotaFiscal && string.IsNullOrWhiteSpace(NumeroNotaFiscal))
            erros.Add("O número da Nota Fiscal deve ser preenchido para entradas por Nota Fiscal.");

        if (Tipo == TipoEntrada.Devolucao)
            NumeroNotaFiscal = null;

        return erros;
    }

    public override void Atualizar(EntidadeBase entidadeAtualizada)
    {
        RequisicaoEntrada requisicaoAtualizada = (RequisicaoEntrada)entidadeAtualizada;

        Produto = requisicaoAtualizada.Produto;
        Quantidade = requisicaoAtualizada.Quantidade;
        Funcionario = requisicaoAtualizada.Funcionario;
        Tipo = requisicaoAtualizada.Tipo;
        NumeroNotaFiscal = requisicaoAtualizada.NumeroNotaFiscal;
    }
}
