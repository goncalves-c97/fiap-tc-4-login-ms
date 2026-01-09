using Core.Helpers;

namespace Test.Core.Helpers;

public class HashHelperTests
{
    [Fact]
    public void ComputeSha256Hash_ReturnsExpectedHash_ForKnownInput()
    {
        var hash = HashHelper.ComputeSha256Hash("abc");
        Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", hash);
    }

    [Fact]
    public void ComputeSha256Hash_ReturnsLowercaseHex_WithLength64()
    {
        var hash = HashHelper.ComputeSha256Hash("anything");
        Assert.Equal(64, hash.Length);
        Assert.Matches("^[0-9a-f]{64}$", hash);
    }
}
