using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UsersController : ControllerBase
{
    private readonly IUserSignupService _userSignupService;

    public UsersController(IUserSignupService userSignupService)
    {
        _userSignupService = userSignupService;
    }

    [HttpGet]
    public ActionResult<string> Test()
    {
        return "hi";
    }

    [HttpPost]
    public async Task<ActionResult> Signup(
        UserSignupRequestDto dto,
        CancellationToken token = default)
    {
        var request = new UserSignupRequest(dto.UserName, dto.Password);

        await _userSignupService.SignupAsync(request, token);

        return Ok();
    }
}