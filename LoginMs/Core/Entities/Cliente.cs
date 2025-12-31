using Core.Helpers;
using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class Cliente : ValidatorClass
{
    public int IdCliente { get; set; }

    public string? Nome { get; set; }

    public string? Email { get; set; }

    public string? Cpf { get; set; }

    public Guid Guid { get; set; } = Guid.NewGuid();

    public Cliente() { }

    public Cliente(string nome, string email, string cpf)
    {
        Nome = nome;
        Email = email;
        Cpf = cpf;
        ValidateValueObjects();
    }

    public void ValidateValueObjects()
    {
        Validate();
    }

    protected override void Validate()
    {
        NotEmptyStringValidation(nameof(Nome), Nome);
        NotEmptyStringValidation(nameof(Email), Email);
        NotEmptyStringValidation(nameof(Cpf), Cpf);
    }
}
