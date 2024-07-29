using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Mappers;

public class OrderItemsMapper : IMapper<ProductsAndOrderItemsDto, IEnumerable<OrderItem>>
{
    public IEnumerable<OrderItem> Map(ProductsAndOrderItemsDto source)
    {
        var orderItems =
            from orderItem in source.OrderItemDtos
            from product in source.Products
            where orderItem.ProductId == product.Id && orderItem.Quantity > 0
            select new OrderItem(product, orderItem.Quantity.Value);

        return orderItems;
    }
}
