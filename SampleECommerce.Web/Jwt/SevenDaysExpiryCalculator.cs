namespace SampleECommerce.Web.Jwt;

public class SevenDaysExpiryCalculator : IJwtExpiryCalculator
{
    public DateTime CalculateExpiry()
        => DateTime.UtcNow.AddDays(7);
}
