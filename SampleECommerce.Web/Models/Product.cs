namespace SampleECommerce.Web.Models;

public class Product
{
    public Product(string id, string name, int quantity, decimal price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentOutOfRangeException.ThrowIfNegative(quantity, nameof(quantity));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price, nameof(price));
        
        Id = id;
        Name = name;
        Quantity = quantity;
        Price = price;
    }

    public string Id { get; }
    
    public string Name { get; }
    
    public int Quantity { get; }
    
    public decimal Price { get; }
}
