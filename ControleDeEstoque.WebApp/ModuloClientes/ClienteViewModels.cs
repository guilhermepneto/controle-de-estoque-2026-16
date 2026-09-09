namespace ControleDeEstoque.WebApp.ModuloClientes;

public record ListarClienteViewModel(
    int Id,
    string Nome,
    string Telefone,
    string Email
);

public record CadastrarClienteViewModel(
    string Nome,
    string Telefone,
    string Email,
    string Cpf
);

public record EditarClienteViewModel(
    int Id,
    string Nome,
    string Telefone,
    string Email,
    string Cpf
);

public record ExcluirClienteViewModel(
    int Id,
    string Nome
);
