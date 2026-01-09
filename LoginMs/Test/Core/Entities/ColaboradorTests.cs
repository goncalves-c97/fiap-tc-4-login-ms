using Core.Entities;
using Core.Helpers;

namespace Test.Core.Entities;

public class ColaboradorTests
{
    [Fact]
    public void Ctor_WhenValid_IsValid()
    {
        var c = new Colaborador(1, "Nome", "email@a.com", "senha");
        Assert.True(c.IsValid);
    }

    [Fact]
    public void Validate_WhenInvalid_RegistersExpectedErrors()
    {
        var c = new Colaborador(0, "", "", "");
        Assert.False(c.IsValid);
        Assert.True(c.ContainsError(GenericErrors.ValueZeroError, nameof(Colaborador.IdFuncao)));
        Assert.True(c.ContainsError(GenericErrors.EmptyStringError, nameof(Colaborador.Nome)));
        Assert.True(c.ContainsError(GenericErrors.EmptyStringError, nameof(Colaborador.Email)));
        Assert.True(c.ContainsError(GenericErrors.EmptyStringError, nameof(Colaborador.Senha)));
    }
}
