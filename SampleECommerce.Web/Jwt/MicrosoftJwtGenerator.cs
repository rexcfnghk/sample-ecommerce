using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace SampleECommerce.Web.Jwt;

public class MicrosoftJwtGenerator(JsonWebTokenHandler jsonWebTokenHandler)
    : IJwtGenerator
{
    private readonly SymmetricSecurityKey
    private readonly JsonWebTokenHandler _jsonWebTokenHandler = jsonWebTokenHandler;

    public string Generate(string userName)
    {
        var signingCredentials = new SigningCredentials()
        _jsonWebTokenHandler.C
    }
}
