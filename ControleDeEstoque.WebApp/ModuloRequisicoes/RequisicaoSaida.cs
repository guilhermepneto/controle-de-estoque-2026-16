using ControleDeEstoque.WebApp.Compartilhado;
using ControleDeEstoque.WebApp.ModuloProdutos;
using ControleDeEstoque.WebApp.ModuloClientes;

namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public class RequisicaoSaida : EntidadeBase
{
    public Cliente Cliente { get; set; } = null!;
    public List<ProdutoPrescrito> ProdutosPrescritos { get; set; } = [];
    public DateTime Data { get; set; } = DateTime.Now;

    public RequisicaoSaida() { }

    public RequisicaoSaida(Cliente cliente, List<ProdutoPrescrito> produtosPrescritos) : this()
    {
        Cliente = cliente;
        ProdutosPrescritos = produtosPrescritos;

        foreach (ProdutoPrescrito mp in ProdutosPrescritos)
            mp.Produto.RegistrarRequisicaoSaida(this);
    }

    public int ObterQuantidade(Produto produto)
    {
        foreach (ProdutoPrescrito mp in ProdutosPrescritos)
        {
            if (mp.Produto.Id == produto.Id)
                return mp.Quantidade;
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

        foreach (ProdutoPrescrito mp in ProdutosPrescritos)
        {
            if (mp.Produto == null)
            {
                erros.Add("O campo \"Produto\" deve ser preenchido.");
            }
            else
            {
                if (mp.Quantidade <= 0)
                    erros.Add($"A \"Quantidade\" do produto \"{mp.Produto.Nome}\" deve ser maior que zero.");

                int estoqueDisponivel = mp.Produto.QuantidadeEmEstoque + mp.Quantidade;

                if (mp.Quantidade > estoqueDisponivel)
                    erros.Add($"Não há estoque suficiente para o produto \"{mp.Produto.Nome}\". Estoque disponível: {estoqueDisponivel}.");
            }
        }

        return erros;
    }

    public override void Atualizar(EntidadeBase entidadeAtualizada)
    {
        RequisicaoSaida requisicaoAtualizada = (RequisicaoSaida)entidadeAtualizada;

        Cliente = requisicaoAtualizada.Cliente;
        ProdutosPrescritos = requisicaoAtualizada.ProdutosPrescritos;
    }
}
