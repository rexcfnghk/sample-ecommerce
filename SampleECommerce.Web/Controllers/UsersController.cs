using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SampleECommerce.Web.Dtos;
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
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Signup(
        UserSignupRequestDto dto,
        CancellationToken token = default)
    {
        var request = new UserSignupRequest(dto.UserName, dto.Password);

        await _userSignupService.SignupAsync(request, token);

        return Created();
    }
}