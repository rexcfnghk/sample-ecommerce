using System.Globalization;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using SampleECommerce.Web.BasicAuthDecoders;
using SampleECommerce.Web.Constants;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.AuthenticationHandlers;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IBasicAuthDecoder basicAuthDecoder,
    IUserRepository userRepository,
    IPasswordEncryptionService passwordEncryptionService)
    : AuthenticationHandler<AuthenticationSchemeOptions>(
        options,
        logger,
        encoder)
{
    private readonly IBasicAuthDecoder _basicAuthDecoder = basicAuthDecoder;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordEncryptionService _passwordEncryptionService =
        passwordEncryptionService;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var request = Context.Request;
        var cancellationToken = Context.RequestAborted;

        var authorization = request.Headers.Authorization;
        if (authorization.Count is 0 or > 1)
        {
            return AuthenticateResult.Fail(
                "Unknown authorization header value");
        }

        var base64EncodedString = RemoveBasicAuthenticationScheme(authorization);
        var dto =
            _basicAuthDecoder.Decode(base64EncodedString);
        if (dto == null)
        {
            return AuthenticateResult.Fail(
                "Cannot decode body into username and password combination");
        }

        var signedUpUser =
            await _userRepository.RetrieveUserAsync(
                dto.UserName,
                cancellationToken);
        if (signedUpUser == null)
        {
            return AuthenticateResult.Fail(
                "Unknown user for given username and password combination");
        }

        var encryptedPassword = _passwordEncryptionService.Encrypt(
            dto.Password,
            signedUpUser.Salt);
        var isPasswordMatched =
            signedUpUser.EncryptedPassword.SequenceEqual(encryptedPassword);

        if (!isPasswordMatched)
        {
            return AuthenticateResult.Fail("Incorrect password for given user");
        }

        var authenticationTicket = GetAuthenticationTicket(signedUpUser.Id);
        return AuthenticateResult.Success(authenticationTicket);
    }

    private static string RemoveBasicAuthenticationScheme(StringValues authorization)
    {
        return authorization.Single()![6..];
    }

    private static AuthenticationTicket GetAuthenticationTicket(int userId)
    {
        var claims = new List<Claim>
        {
            new(
                ClaimNames.UserId,
                userId.ToString(CultureInfo.InvariantCulture)),
        };
        var identity = new ClaimsIdentity(
            claims,
            AuthenticationSchemes.Basic);
        var principal = new ClaimsPrincipal(identity);
        var authenticationTicket =
            new AuthenticationTicket(
                principal,
                AuthenticationSchemes.Basic);
        return authenticationTicket;
    }
}
