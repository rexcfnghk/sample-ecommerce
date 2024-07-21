using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Jwt;

namespace SampleECommerce.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionsController(IJwtGenerator jwtGenerator) : ControllerBase
{
    private readonly IJwtGenerator _jwtGenerator = jwtGenerator;
    
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<JwtTokenDto>> CreateSession(
        UserRequestDto dto,
        CancellationToken token = default)
    {
        var jwt = _jwtGenerator.Generate(dto.UserName);
        
        return new JwtTokenDto(jwt);
    }
}
