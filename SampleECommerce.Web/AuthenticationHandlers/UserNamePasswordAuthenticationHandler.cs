using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Serializers;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.AuthenticationHandlers;

public class UserNamePasswordAuthenticationHandler(
    IPasswordEncryptionService passwordEncryptionService,
    IUserRepository userRepository,
    ISerializer serializer,
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(
        options,
        logger,
        encoder)
{
    private readonly IPasswordEncryptionService _passwordEncryptionService =
        passwordEncryptionService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISerializer _serializer = serializer;
    
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            var claimsPrincipal =
                new ClaimsPrincipal(Context.User);
            var anonymousTicket = new AuthenticationTicket(
                claimsPrincipal,
                "Anonymous");
            return AuthenticateResult.Success(anonymousTicket);
        }
        
        var requestDto =
            await _serializer.DeserializeAsync<UserRequestDto>(Request.Body);
        if (requestDto is null)
        {
            return AuthenticateResult.Fail("Malformed request body for authentication");
        }

        var signedUpUser =
            await _userRepository.RetrieveUserAsync(requestDto.UserName, Context.RequestAborted);
        if (signedUpUser is null)
        {
            return AuthenticateResult.Fail("Not a signed up user");
        }

        var encryptedPassword = _passwordEncryptionService.Encrypt(
            requestDto.Password,
            signedUpUser.Salt);

        if (!signedUpUser.EncryptedPassword.SequenceEqual(encryptedPassword))
        {
            return AuthenticateResult.Fail("Password mismatch");
        }

        var authenticationTicket = BuildAuthenticationTicket(requestDto);
        return AuthenticateResult.Success(authenticationTicket);
    }

    private AuthenticationTicket BuildAuthenticationTicket(
        UserRequestDto requestDto)
    {
        var claims = new List<Claim>
            { new(ClaimTypes.Name, requestDto.UserName) };
        var claimsPrincipal =
            new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
        var authenticationTicket =
            new AuthenticationTicket(claimsPrincipal, Scheme.Name);
        return authenticationTicket;
    }
}
