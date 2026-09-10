using System.Text.RegularExpressions;
using ControleDeEstoque.WebApp.Compartilhado;

namespace ControleDeEstoque.WebApp.ModuloClientes;

public enum TipoDocumentoCliente
{
    CPF,
    CNPJ
}

public class Cliente : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public TipoDocumentoCliente TipoDocumento { get; set; } = TipoDocumentoCliente.CPF;

    public string Documento => Cpf;

    public Cliente() { }

    public Cliente(string nome, string telefone, string email, string documento, TipoDocumentoCliente tipoDocumento = TipoDocumentoCliente.CPF) : this()
    {
        Nome = nome;
        Telefone = telefone;
        Email = email;
        Cpf = documento;
        TipoDocumento = tipoDocumento;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Nome) || Nome.Length < 3 || Nome.Length > 100)
            erros.Add("O campo \"Nome\" deve conter entre 3 e 100 caracteres.");

        if (!Regex.IsMatch(Telefone, @"^\(\d{2}\) \d{4,5}-\d{4}$"))
            erros.Add("O campo \"Telefone\" deve estar no formato (XX) XXXX-XXXX ou (XX) XXXXX-XXXX.");

        if (!string.IsNullOrWhiteSpace(Email) && !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            erros.Add("O E-mail deve ser válido.");

        if (TipoDocumento == TipoDocumentoCliente.CPF)
        {
            if (!Regex.IsMatch(Cpf, @"^\d{11}$"))
                erros.Add("O CPF deve conter exatamente 11 dígitos.");
        }
        else if (!Regex.IsMatch(Cpf, @"^\d{14}$"))
        {
            erros.Add("O CNPJ deve conter exatamente 14 dígitos.");
        }

        return erros;
    }

    public override void Atualizar(EntidadeBase entidadeAtualizada)
    {
        Cliente clienteAtualizado = (Cliente)entidadeAtualizada;

        Nome = clienteAtualizado.Nome;
        Telefone = clienteAtualizado.Telefone;
        Email = clienteAtualizado.Email;
        Cpf = clienteAtualizado.Cpf;
        TipoDocumento = clienteAtualizado.TipoDocumento;
    }
}
