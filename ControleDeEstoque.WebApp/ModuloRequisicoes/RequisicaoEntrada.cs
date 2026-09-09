using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.ModuloFuncionario;
using ControleDeEstoque.WebApp.ModuloProdutos;

namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public class RequisicaoEntrada : EntidadeBase
{
    public Produto Produto { get; set; } = null!;
    public Funcionario Funcionario { get; set; } = null!;
    public int Quantidade { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;

    public RequisicaoEntrada() { }

    public RequisicaoEntrada(Produto produto, int quantidade, Funcionario funcionario) : this()
    {
        Produto = produto;
        Quantidade = quantidade;
        Funcionario = funcionario;

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

        return erros;
    }

    public override void Atualizar(EntidadeBase entidadeAtualizada)
    {
        RequisicaoEntrada requisicaoAtualizada = (RequisicaoEntrada)entidadeAtualizada;

        Produto = requisicaoAtualizada.Produto;
        Quantidade = requisicaoAtualizada.Quantidade;
        Funcionario = requisicaoAtualizada.Funcionario;
    }
}
