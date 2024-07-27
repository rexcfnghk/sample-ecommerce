using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SampleECommerce.Web.Constants;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Serializers;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.AuthenticationHandlers;

public class UserNamePasswordAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IUserRepository userRepository,
    ISerializer serializer,
    IPasswordEncryptionService passwordEncryptionService)
    : AuthenticationHandler<AuthenticationSchemeOptions>(
        options,
        logger,
        encoder)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISerializer _serializer = serializer;
    private readonly IPasswordEncryptionService _passwordEncryptionService =
        passwordEncryptionService;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var request = Context.Request;
        var cancellationToken = Context.RequestAborted;
        request.EnableBuffering();

        using var streamReader = new StreamReader(
            request.Body,
            Encoding.UTF8,
            bufferSize: -1,
            leaveOpen: true);

        var dto = await _serializer.DeserializeAsync<UserRequestDto>(
            streamReader.BaseStream,
            cancellationToken: cancellationToken);
        if (dto == null)
        {
            return AuthenticateResult.Fail(
                "Cannot deserialize body into username and password combination");
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

        request.Body.Seek(0, SeekOrigin.Begin);
        return AuthenticateResult.Success(authenticationTicket);
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
            AuthenticationSchemes.UserNamePassword);
        var principal = new ClaimsPrincipal(identity);
        var authenticationTicket =
            new AuthenticationTicket(
                principal,
                AuthenticationSchemes.UserNamePassword);
        return authenticationTicket;
    }
}
