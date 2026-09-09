namespace ControleDeEstoque.WebApp.Compartilhado;

public abstract class EntidadeBase
{
    public int Id { get; set; }

    public abstract List<string> Validar();
    public abstract void Atualizar(EntidadeBase entidadeAtualizada);
}
