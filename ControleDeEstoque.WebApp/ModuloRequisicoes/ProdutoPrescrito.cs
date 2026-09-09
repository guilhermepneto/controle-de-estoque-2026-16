using ControleDeEstoque.WebApp.ModuloProdutos;

namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public class ProdutoPrescrito
{
    public Produto Produto { get; set; } = null!;
    public int Quantidade { get; set; }

    public ProdutoPrescrito() { }

    public ProdutoPrescrito(Produto produto, int quantidade)
    {
        Produto = produto;
        Quantidade = quantidade;
    }
}
