namespace SampleECommerce.Web.Dtos;

public class OrderItemDto
{
    public Guid OrderItemId { get; set; }
    
    public string ProductName { get; set; }
    
    public int Quantity { get; set; }

}
