using Core.Entities;
using Core.Helpers;

namespace Test.Core.Entities;

public class ClienteTests
{
    [Fact]
    public void Ctor_WhenValid_SetsPropertiesAndIsValid()
    {
        var c = new Cliente("Nome", "email@a.com", "123");
        Assert.True(c.IsValid);
        Assert.Equal("Nome", c.Nome);
    }

    [Fact]
    public void Validate_WhenMissingFields_RegisterErrors()
    {
        var c = new Cliente { Nome = null, Email = null, Cpf = null };
        c.ValidateValueObjects();

        Assert.False(c.IsValid);
        Assert.True(c.ContainsError(GenericErrors.EmptyStringError, nameof(Cliente.Nome)));
        Assert.True(c.ContainsError(GenericErrors.EmptyStringError, nameof(Cliente.Email)));
        Assert.True(c.ContainsError(GenericErrors.EmptyStringError, nameof(Cliente.Cpf)));
    }
}
