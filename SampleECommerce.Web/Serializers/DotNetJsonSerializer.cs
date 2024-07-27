using System.Text.Json;

namespace SampleECommerce.Web.Serializers;

public class DotNetJsonSerializer(JsonSerializerOptions jsonSerializerOptions) : ISerializer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions =
        jsonSerializerOptions;

    public ValueTask<T?> DeserializeAsync<T>(
        Stream stream,
        CancellationToken cancellationToken = default)
        => JsonSerializer.DeserializeAsync<T>(
            stream,
            _jsonSerializerOptions,
            cancellationToken);
}
