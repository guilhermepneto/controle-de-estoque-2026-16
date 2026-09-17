using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.ModuloProdutos;
using ControleDeEstoque.WebApp.ModuloClientes;

namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public class RequisicaoSaida : EntidadeBase
{
    public Cliente Cliente { get; set; } = null!;
    public List<ProdutoPrescrito> ProdutosPrescritos { get; set; } = [];
    public DateTime Data { get; set; } = DateTime.Now;

    public RequisicaoSaida()
    {
    }

    public RequisicaoSaida(
        Cliente cliente,
        List<ProdutoPrescrito> produtosPrescritos) : this()
    {
        Cliente = cliente;
        ProdutosPrescritos = produtosPrescritos;

        foreach (ProdutoPrescrito produtoPrescrito in ProdutosPrescritos)
            produtoPrescrito.Produto.RegistrarRequisicaoSaida(this);
    }

    public int ObterQuantidade(Produto produto)
    {
        foreach (ProdutoPrescrito produtoPrescrito in ProdutosPrescritos)
        {
            if (produtoPrescrito.Produto.Id == produto.Id)
                return produtoPrescrito.Quantidade;
        }

        return 0;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (Cliente == null)
            erros.Add("O campo \"Cliente\" deve ser preenchido.");

        if (ProdutosPrescritos.Count == 0)
            erros.Add("É necessário selecionar ao menos um produto.");

        foreach (ProdutoPrescrito produtoPrescrito in ProdutosPrescritos)
        {
            if (produtoPrescrito.Produto == null)
            {
                erros.Add("O campo \"Produto\" deve ser preenchido");
                continue;
            }

            if (produtoPrescrito.Quantidade <= 0)
            {
                erros.Add(
                    $"A \"Quantidade\" do produto \"{produtoPrescrito.Produto.Nome}\" deve ser maior que zero."
                );

                continue;
            }

            int estoqueDisponivel = produtoPrescrito.Produto.QuantidadeEmEstoque;

            if (produtoPrescrito.Quantidade > estoqueDisponivel)
            {
                erros.Add(
                    $"Não há estoque suficiente para o produto \"{produtoPrescrito.Produto.Nome}\". " +
                    $"Estoque disponível: {estoqueDisponivel}."
                );
            }
        }

        return erros;
    }

    public override void Atualizar(EntidadeBase entidadeAtualizada)
    {
        RequisicaoSaida requisicaoAtualizada =
            (RequisicaoSaida)entidadeAtualizada;

        Cliente = requisicaoAtualizada.Cliente;
        ProdutosPrescritos = requisicaoAtualizada.ProdutosPrescritos;
    }
}