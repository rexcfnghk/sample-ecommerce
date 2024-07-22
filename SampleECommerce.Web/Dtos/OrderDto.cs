namespace SampleECommerce.Web.Dtos;

public class OrderDto
{
    public DateTimeOffset OrderTime { get; set; }

    public IList<OrderItemDto> OrderItems { get; set; } =
        new List<OrderItemDto>();
}
