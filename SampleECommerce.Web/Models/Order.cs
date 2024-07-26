using Microsoft.AspNetCore.Routing.Matching;
using SampleECommerce.Web.Repositories;

namespace SampleECommerce.Web.Models;

public class Order
{
    public Order(Guid id, DateTimeOffset orderTime, IReadOnlyList<OrderItem> orderItems)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty, nameof(id));
        ArgumentNullException.ThrowIfNull(orderItems, nameof(orderItems));
        ArgumentOutOfRangeException.ThrowIfZero(orderItems.Count, nameof(orderItems.Count));
        
        Id = id;
        OrderTime = orderTime;
        OrderItems = orderItems;
    }

    public Guid Id { get; }
    public DateTimeOffset OrderTime { get; }
    public IReadOnlyList<OrderItem> OrderItems { get; }

    public decimal OrderSum => OrderItems.Sum(oi => oi.Quantity * oi.Product.Price);

}
