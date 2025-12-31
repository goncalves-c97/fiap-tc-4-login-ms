using Core.Helpers;

namespace Core.Entities;

public class Colaborador : ValidatorClass
{
    public int IdColaborador { get; set; }
    public int IdFuncao { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }

    public virtual FuncaoColaborador IdFuncaoNavigation { get; set; } = null!;

    public Colaborador() { }

    public Colaborador(int idFuncao, string nome, string email, string senha = "")
    {
        IdFuncao = idFuncao;
        Nome = nome;
        Email = email;
        Senha = senha;
        ValidateValueObjects();
    }

    public void ValidateValueObjects()
    {
        Validate();
    }

    protected override void Validate()
    {
        PositiveValueValidation(nameof(IdFuncao), IdFuncao);
        NotEmptyStringValidation(nameof(Nome), Nome);
        NotEmptyStringValidation(nameof(Email), Email);
        NotEmptyStringValidation(nameof(Senha), Senha);
    }
}
