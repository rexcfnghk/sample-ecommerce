using Microsoft.AspNetCore.Mvc;
using SampleECommerce.Web.ModelBinders;

namespace SampleECommerce.Web.Dtos;

[ModelBinder<UserDtoModelBinder>]
public class UserIdDto
{
    public int UserId { get; set; }
}
