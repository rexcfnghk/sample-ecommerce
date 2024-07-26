namespace SampleECommerce.Web.Services;

public interface IOrderTimeGenerator
{
    DateTimeOffset GenerateOrderTime();
}
