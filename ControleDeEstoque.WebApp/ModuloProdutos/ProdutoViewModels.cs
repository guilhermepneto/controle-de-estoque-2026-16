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
    int QuantidadeEmEstoque
);

public record CadastrarProdutoViewModel(
    string Nome,
    string Descricao,
    int FornecedorId
)
{
    public List<FornecedorProdutoViewModel> Fornecedores { get; init; } = [];
}

public record EditarProdutoViewModel(
    int Id,
    string Nome,
    string Descricao,
    int FornecedorId
)
{
    public List<FornecedorProdutoViewModel> Fornecedores { get; init; } = [];
}
