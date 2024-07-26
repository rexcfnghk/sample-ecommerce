namespace SampleECommerce.Web.Services;

public sealed class GuidOrderIdGenerator : IOrderIdGenerator
{
    public Guid GenerateOrderId()
        => Guid.NewGuid();
}
