using System.ComponentModel.DataAnnotations;

namespace SampleECommerce.Web.Dtos;

public class PostOrderItemDto
{
    [Required]
    public string? ProductId { get; set; }
    
    [Range(0, int.MaxValue)]
    [Required]
    public int? Quantity { get; set; }
}
