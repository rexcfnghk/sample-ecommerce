using System.Text.Json;

namespace SampleECommerce.Web.Serializers;

public class CatchJsonExceptionSerializer(ISerializer decoratee) : ISerializer
{
    private readonly ISerializer _decoratee = decoratee;

    public async ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _decoratee.DeserializeAsync<T>(stream, cancellationToken);
        }
        catch (JsonException)
        {
            return default;
        }
    }
}
