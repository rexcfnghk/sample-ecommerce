namespace SampleECommerce.Web.Services;

public sealed class CurrentDateTimeOffsetOrderTimeGenerator : IOrderTimeGenerator
{
    public DateTimeOffset GenerateOrderTime()
        => DateTimeOffset.Now;
}
