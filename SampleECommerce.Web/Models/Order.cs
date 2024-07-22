namespace SampleECommerce.Web.Models;

public record Order(Guid Id, DateTimeOffset OrderTime, IReadOnlyList<OrderItem> OrderItems);
