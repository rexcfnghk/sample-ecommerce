using System.ComponentModel.DataAnnotations;

namespace SampleECommerce.Web.Dtos;

public class PostOrderDto
{
    [Required]
    public IList<PostOrderItemDto> OrderItems { get; set; } =
        new List<PostOrderItemDto>();
}
