using System.Text.Json;
using Core.Adapters;

namespace Test.Core.Adapters;

public class ObjectToJsonStringAdapterTests
{
    [Fact]
    public void ConvertToJsonString_WhenJsonElement_ReturnsElementString()
    {
        using var doc = JsonDocument.Parse("{\"a\":1}");
        var json = ObjectToJsonStringAdapter.ConvertToJsonString(doc.RootElement);
        Assert.Equal("{\"a\":1}", json);
    }

    [Fact]
    public void ConvertToJsonString_WhenPoco_ReturnsSerializedJson()
    {
        var json = ObjectToJsonStringAdapter.ConvertToJsonString(new { A = 1 });
        Assert.Contains("\"A\":1", json);
    }
}
