namespace ControleDeEstoque.WebApp.ModuloFuncionario
{
    public record ListarFuncionarioViewModel(
        int Id,
        string Nome,
        string Telefone
        );

    public record CadastrarFuncionarioViewModel(
        string Nome,
        string Telefone,
        string Cpf
    );

    public record EditarFuncionarioViewModel(
        int Id,
        string Nome,
        string Telefone,
        string Cpf
    );

    public record ExcluirFuncionarioViewModel(
        int Id,
        string Nome
    );

}
