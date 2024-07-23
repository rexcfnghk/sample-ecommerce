namespace SampleECommerce.Web.Models;

public class OrderItem
{
    public OrderItem(Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product, nameof(product));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));
        
        Product = product;
        Quantity = quantity;
    }

    public Product Product { get; }
    public int Quantity { get; }
}
