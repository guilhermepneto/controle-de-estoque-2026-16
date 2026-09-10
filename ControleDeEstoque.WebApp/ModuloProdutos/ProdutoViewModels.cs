namespace ControleDeEstoque.WebApp.ModuloProdutos;

public record FornecedorProdutoViewModel(
    int Id,
    string Nome
);

public record ListarProdutoViewModel(
    int Id,
    string Nome,
    string Descricao,
    string NomeFornecedor,
    int QuantidadeEmEstoque,
    CategoriaProduto Categoria
);

public record CadastrarProdutoViewModel(
    string Nome,
    string Descricao,
    int FornecedorId,
    CategoriaProduto Categoria
)
{
    public List<FornecedorProdutoViewModel> Fornecedores { get; init; } = [];
}

public record EditarProdutoViewModel(
    int Id,
    string Nome,
    string Descricao,
    int FornecedorId,
    CategoriaProduto Categoria
)
{
    public List<FornecedorProdutoViewModel> Fornecedores { get; init; } = [];
}
