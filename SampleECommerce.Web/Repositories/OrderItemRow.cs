namespace SampleECommerce.Web.Repositories;

public record OrderItemRow(
    Guid Id,
    Guid OrderId,
    string ProductId,
    string ProductName,
    int ProductQuantity,
    decimal ProductPrice,
    string ProductCategory,
    int OrderQuantity,
    DateTimeOffset OrderTime);
