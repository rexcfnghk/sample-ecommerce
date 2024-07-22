namespace SampleECommerce.Web.Models;

public record OrderItem(
    Guid Id,
    Guid OrderId,
    string ProductName,
    int Quantity,
    DateTimeOffset OrderTime);
