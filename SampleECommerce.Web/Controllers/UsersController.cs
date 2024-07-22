using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.ModelBinders;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserSignupService userSignupService)
    : ControllerBase
{
    private readonly IUserSignupService _userSignupService = userSignupService;

    [HttpPost]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Signup(
        UserRequestDto dto,
        CancellationToken token = default)
    {
        var request = new UserSignupRequest(dto.UserName, dto.Password);

        await _userSignupService.SignupAsync(request, token);

        return NoContent();
    }
    
    [HttpGet("orders")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> ListOrders(UserIdDto userId)
    {
        return string.Empty;
    }
}
