using System.Text.RegularExpressions;
using ControleDeEstoque.WebApp.Compartilhado;

namespace ControleDeEstoque.WebApp.ModuloClientes;

public class Cliente : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;

    public Cliente()
    {
    }

    public Cliente(string nome, string telefone, string email, string cpf) : this()
    {
        Nome = nome;
        Telefone = telefone;
        Email = email;
        Cpf = cpf;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Nome) || Nome.Length < 3 || Nome.Length > 100)
            erros.Add("O campo \"Nome\" deve conter entre 3 e 100 caracteres.");

        if (!Regex.IsMatch(Telefone, @"^\(\d{2}\) \d{4,5}-\d{4}$"))
            erros.Add("O campo \"Telefone\" deve estar no formato (XX) XXXX-XXXX ou (XX) XXXXX-XXXX.");

        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            erros.Add("O E-mail deve ser válido.");

        if (!Regex.IsMatch(Cpf, @"^\d{11}$"))
            erros.Add("O CPF deve conter exatamente 11 dígitos.");

        return erros;
    }

    public override void Atualizar(EntidadeBase entidadeAtualizada)
    {
        Cliente clienteAtualizado = (Cliente)entidadeAtualizada;

        Nome = clienteAtualizado.Nome;
        Telefone = clienteAtualizado.Telefone;
        Email = clienteAtualizado.Email;
        Cpf = clienteAtualizado.Cpf;
    }

}
