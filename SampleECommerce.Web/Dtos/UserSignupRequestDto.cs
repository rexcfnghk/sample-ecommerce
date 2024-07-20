using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SampleECommerce.Web.Dtos;

public class UserSignupRequestDto
{
    [Required, NotNull]
    public string? UserName { get; set; }
    
    [Required, NotNull]
    public string? Password { get; set; }   
}