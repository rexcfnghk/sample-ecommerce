using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Jwt;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionsController(
    IUserRepository userRepository, 
    IPasswordEncryptionService passwordEncryptionService,
    IJwtGenerator jwtGenerator) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;

    private readonly IPasswordEncryptionService _passwordEncryptionService =
        passwordEncryptionService;
    private readonly IJwtGenerator _jwtGenerator = jwtGenerator;
    
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        
        var jwt = _jwtGenerator.Generate(dto.UserName);
        return new JwtTokenDto(jwt);
    }
}
