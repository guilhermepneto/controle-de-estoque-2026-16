namespace ControleDeEstoque.WebApp.ModuloRequisicoes;

public record ProdutoRequisicaoEntradaViewModel(
    int Id,
    string Nome
);

public record FuncionarioRequisicaoEntradaViewModel(
    int Id,
    string Nome
);

public record ListarRequisicaoEntradaViewModel(
    int Id,
    string NomeProduto,
    string NomeFuncionario,
    int Quantidade,
    DateTime Data,
    TipoEntrada Tipo,
    string? NumeroNotaFiscal
);

public record CadastrarRequisicaoEntradaViewModel(
    int ProdutoId,
    int FuncionarioId,
    int Quantidade,
    TipoEntrada Tipo = TipoEntrada.NotaFiscal,
    string? NumeroNotaFiscal = null
)
{
    public List<ProdutoRequisicaoEntradaViewModel> Produtos { get; init; } = [];
    public List<FuncionarioRequisicaoEntradaViewModel> Funcionarios { get; init; } = [];
}
