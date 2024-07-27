namespace SampleECommerce.Web.Serializers;

public interface ISerializer
{
    ValueTask<T?> DeserializeAsync<T>(
        Stream stream,
        CancellationToken cancellationToken = default);
}
