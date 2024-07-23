namespace SampleECommerce.Web.Dtos;

public class OrderItemDto
{
    public string ProductName { get; set; }
    
    public decimal ProductPrice { get; set; }
    
    public string ProductCategory { get; set; }
    
    public int Quantity { get; set; }

}
