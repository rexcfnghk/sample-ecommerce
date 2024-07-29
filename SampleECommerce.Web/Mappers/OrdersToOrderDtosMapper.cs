using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Mappers;

public class OrdersToOrderDtosMapper : IMapper<IEnumerable<Order>, Dictionary<Guid, OrderDto>>
{
    public Dictionary<Guid, OrderDto> Map(IEnumerable<Order> source)
    {
        var output = source.ToDictionary(
            dto => dto.Id,
            dto => new OrderDto
            {
                OrderTime = dto.OrderTime,
                OrderItems = dto.OrderItems.Select(
                        oi => new OrderItemDto
                        {
                            ProductName = oi.Product.Name,
                            Quantity = oi.Quantity,
                            ProductCategory = oi.Product.Category,
                            ProductPrice = oi.Product.Price
                        })
                    .ToList()
            });

        return output;
    }
}
