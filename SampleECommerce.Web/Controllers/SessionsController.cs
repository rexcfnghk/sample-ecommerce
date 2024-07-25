using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Jwt;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Controllers;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class SessionsController(
    IUserRepository userRepository, 
    IPasswordEncryptionService passwordEncryptionService,
    IJwtGenerator jwtGenerator) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;

    private readonly IPasswordEncryptionService _passwordEncryptionService =
        passwordEncryptionService;
    private readonly IJwtGenerator _jwtGenerator = jwtGenerator;
    
    /// <summary>
    /// Creates a session for the user
    /// </summary>
    /// <param name="dto">The username and password combination for the user</param>
    /// <param name="token"></param>
    /// <response code="200">A session is successfully created</response>
    /// <response code="400">When there are missing field(s), or the request is malformed</response>
    /// <response code="401">The system cannot authenticate the user</response>
    /// <returns>The order that the user made</returns>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public async Task<ActionResult<JwtTokenDto>> CreateSession(
        UserRequestDto dto,
        CancellationToken token = default)
    {
        var signedUpUser =
            await _userRepository.RetrieveUserAsync(dto.UserName, token);
        if (signedUpUser is null)
        {
            return Unauthorized("Unknown user");
        }

        var encryptedPassword = _passwordEncryptionService.Encrypt(
            dto.Password,
            signedUpUser.Salt);

        if (!signedUpUser.EncryptedPassword.SequenceEqual(encryptedPassword))
        {
            return Unauthorized("Incorrect password");
        }
        
        var jwt = _jwtGenerator.Generate(signedUpUser.Id);
        return new JwtTokenDto(jwt);
    }
}
