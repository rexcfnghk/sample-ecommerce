namespace SampleECommerce.Web.Jwt;

public interface IJwtExpiryCalculator
{
    DateTime CalculateExpiry();
}
