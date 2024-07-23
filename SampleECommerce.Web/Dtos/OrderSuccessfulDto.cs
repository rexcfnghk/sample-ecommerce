namespace SampleECommerce.Web.Dtos;

public class OrderSuccessfulDto
{
    public Guid OrderId { get; set; }
    
    public DateTimeOffset OrderTime { get; set; }
}
