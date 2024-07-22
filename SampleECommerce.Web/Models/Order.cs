namespace SampleECommerce.Web.Models;

public record Order(
    Guid Id,
    string ProductName,
    int Quantity,
    DateTimeOffset OrderTime);
