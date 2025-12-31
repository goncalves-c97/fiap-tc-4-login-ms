namespace Core.Entities;

public partial class FuncaoColaborador
{
    public int IdFuncao { get; set; }

    public string Funcao { get; set; } = null!;

    public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
