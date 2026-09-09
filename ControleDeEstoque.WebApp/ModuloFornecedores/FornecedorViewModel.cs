using System.ComponentModel.DataAnnotations;

namespace ControleDeEstoque.WebApp.ModuloFornecedores
{
    public record ListarFornecedorViewModel(
        int Id,
        string Nome,
        string Telefone,
        string Cnpj
        );

    public record CadastrarFornecedorViewModel(
        [Required(ErrorMessage = "O campo \"Nome\" é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Nome\" deve conter entre 2 e 100 caracteres.")]
        string Nome,

        [Required(ErrorMessage = "O campo \"Telefone\" é obrigatório")]
        [RegularExpression(@"^\(\d{2}\) \d{4,5}-\d{4}$", ErrorMessage = "O campo \"Telefone\" deve estar no formato (DDD) 90000-0000.")]
        string Telefone,

        [Required(ErrorMessage = "O campo \"Cnpj\" é obrigatório")]
        [RegularExpression(@"^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}-?\d{2}$", ErrorMessage = "O campo \"CNPJ\" deve conter 14 dígitos.")]
        string Cnpj
    );

    public record EditarFornecedorViewModel(
        int Id,

        [Required(ErrorMessage = "O campo \"Nome\" é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Nome\" deve conter entre 2 e 100 caracteres.")]
        string Nome,

        [Required(ErrorMessage = "O campo \"Telefone\" é obrigatório")]
        [RegularExpression(@"^\(\d{2}\) \d{4,5}-\d{4}$", ErrorMessage = "O campo \"Telefone\" deve estar no formato (DDD) 90000-0000.")]
        string Telefone,

        [Required(ErrorMessage = "O campo \"Cnpj\" é obrigatório")]
        [RegularExpression(@"^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}-?\d{2}$", ErrorMessage = "O campo \"CNPJ\" deve conter 14 dígitos.")]
        string Cnpj
    );

    public record ExcluirFornecedorViewModel(
        int Id,
        string Nome
    );
}
