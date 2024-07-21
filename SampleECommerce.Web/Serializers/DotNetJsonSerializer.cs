using System.Text.Json;

namespace SampleECommerce.Web.Serializers;

public class DotNetJsonSerializer : ISerializer
{
    public ValueTask<T?> DeserializeAsync<T>(Stream stream)
        => JsonSerializer.DeserializeAsync<T>(stream);
}
