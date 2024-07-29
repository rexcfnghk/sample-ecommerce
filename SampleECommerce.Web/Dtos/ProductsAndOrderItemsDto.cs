using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Dtos;

public record ProductsAndOrderItemsDto(
    IEnumerable<Product> Products,
    IEnumerable<PostOrderItemDto> OrderItemDtos);
