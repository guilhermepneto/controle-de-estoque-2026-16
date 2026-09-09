namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public record ListarProdutoPrescritoRequisicaoSaidaViewModel(
    int Id,
    string Nome,
    int Quantidade
);

public record ListarRequisicaoSaidaViewModel(
    int Id,
    string NomeCliente,
    DateTime Data,
    List<ListarProdutoPrescritoRequisicaoSaidaViewModel> ProdutosPrescritos
);


public record ClienteRequisicaoSaidaViewModel(
    int Id,
    string Nome
);

public record ProdutoPrescritoRequisicaoSaidaViewModel(
    int ProdutoId,
    string NomeProduto,
    int QuantidadeEmEstoque,
    bool Selecionado,
    int Quantidade
);

public record CadastrarRequisicaoSaidaViewModel(int ClienteId)
{
    public List<ClienteRequisicaoSaidaViewModel> Clientes { get; init; } = [];
    public List<ProdutoPrescritoRequisicaoSaidaViewModel> ProdutosPrescritos { get; init; } = [];
}
