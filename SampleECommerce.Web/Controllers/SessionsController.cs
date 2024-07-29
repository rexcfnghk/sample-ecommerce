using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleECommerce.Web.Constants;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Jwt;
using SampleECommerce.Web.Mappers;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Controllers;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class SessionsController(IMapper<ClaimsPrincipal, UserId> userIdMapper, IJwtGenerator jwtGenerator) : ControllerBase
{
    private readonly IMapper<ClaimsPrincipal, UserId> _userIdMapper =
        userIdMapper;
    private readonly IJwtGenerator _jwtGenerator = jwtGenerator;
    
    /// <summary>
    /// Creates a session for the user
    /// </summary>
    /// <response code="200">A session is successfully created</response>
    /// <response code="400">When there are missing field(s), or the request is malformed</response>
    /// <response code="401">The system cannot authenticate the user</response>
    /// <returns>The order that the user made</returns>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Basic)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public ActionResult<JwtTokenDto> CreateSession()
    {
        var userId = _userIdMapper.Map(User);
        var jwt = _jwtGenerator.Generate(userId);
        return new JwtTokenDto(jwt);
    }
}
