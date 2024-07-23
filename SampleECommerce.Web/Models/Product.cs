namespace SampleECommerce.Web.Models;

public class Product
{
    public Product(string id, string name, int quantity, decimal price, string category)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentOutOfRangeException.ThrowIfNegative(quantity, nameof(quantity));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price, nameof(price));
        ArgumentException.ThrowIfNullOrWhiteSpace(category, nameof(category));
        
        Id = id;
        Name = name;
        Quantity = quantity;
        Price = price;
        Category = category;
    }

    public string Id { get; }
    
    public string Name { get; }
    
    public int Quantity { get; }
    
    public decimal Price { get; }
    
    public string Category { get; }
}
