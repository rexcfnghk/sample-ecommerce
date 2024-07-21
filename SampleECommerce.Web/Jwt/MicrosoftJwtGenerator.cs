using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SampleECommerce.Web.Serializers;

namespace SampleECommerce.Web.Jwt;

public class MicrosoftJwtGenerator(
    JwtIssuer issuer,
    IJwtExpiryCalculator expiryCalculator,
    SigningCredentials signingCredentials,
    JsonWebTokenHandler jsonWebTokenHandler)
    : IJwtGenerator
{
    private readonly JwtIssuer _issuer = issuer;
    private readonly IJwtExpiryCalculator _expiryCalculator = expiryCalculator;

    private readonly SigningCredentials
        _signingCredentials = signingCredentials;

    private readonly JsonWebTokenHandler _jsonWebTokenHandler =
        jsonWebTokenHandler;

    public string Generate(string userName)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer.Issuer,
            Audience = userName,
            Expires = _expiryCalculator.CalculateExpiry(),
            SigningCredentials = _signingCredentials
        };

        var token = _jsonWebTokenHandler.CreateToken(tokenDescriptor);
        return token;
    }
}
