namespace ApiCrud.Estudantes;

public class Estudante
{
    public Guid Id { get; init; }
    public string Nome { get; private set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }

    public Estudante(string nome)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Ativo = true;
        DataCriacao = DateTime.Now;
    }

    public void AtualizarNome(string nome)
    {
        Nome = nome;
    }

    public void Desativar()
    {
        Ativo = false;
    }
}