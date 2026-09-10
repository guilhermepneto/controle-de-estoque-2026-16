using ControleDeEstoque.WebApp.ModuloRequisicoes;

namespace ControleDeEstoque.WebApp.ModuloEstoque;

public record ListarEstoqueViewModel(
    int Id,
    string NomeProduto,
    string Categoria,
    int QuantidadeEmEstoque
);

public record RelatorioEntradaViewModel(
    DateTime Data,
    string NomeProduto,
    int Quantidade,
    string Tipo,
    string Documento,
    string NomeFuncionario
);

public record RelatorioSaidaViewModel(
    DateTime Data,
    string NomeCliente,
    string NomeProduto,
    int Quantidade
);

public record RelatorioSemanalViewModel(
    DateTime Inicio,
    DateTime Fim,
    List<RelatorioEntradaViewModel> Entradas,
    List<RelatorioSaidaViewModel> Saidas
);
