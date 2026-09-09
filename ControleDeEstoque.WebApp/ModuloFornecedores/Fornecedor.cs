using System.Text.RegularExpressions;
using ControleDeEstoque.WebApp.Compartilhado;

namespace ControleDeEstoque.WebApp.ModuloFornecedores;

public class Fornecedor : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;

    public Fornecedor() { }

    public Fornecedor(string nome, string telefone, string cnpj) : this()
    {
        Nome = nome;
        Telefone = telefone;
        Cnpj = cnpj;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Nome) || Nome.Length < 2 || Nome.Length > 100)
            erros.Add("O campo \"Nome\" deve conter entre 2 e 100 caracteres.");

        if (!Regex.IsMatch(Telefone, @"^\(\d{2}\) \d{4,5}-\d{4}$"))
            erros.Add("O campo \"Telefone\" deve estar no formato (DDD) 90000-0000.");

        if (!Regex.IsMatch(Cnpj, @"^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}-?\d{2}$"))
            erros.Add("O campo \"CNPJ\" deve conter 14 dígitos.");

        return erros;
    }

    public override void Atualizar(EntidadeBase entidadeAtualizada)
    {
        Fornecedor fornecedorAtualizado = (Fornecedor)entidadeAtualizada;

        Nome = fornecedorAtualizado.Nome;
        Telefone = fornecedorAtualizado.Telefone;
        Cnpj = fornecedorAtualizado.Cnpj;
    }
}
