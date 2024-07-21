using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace SampleECommerce.Web.Jwt;

public class MicrosoftJwtGenerator//(SymmetricSecurityKey securityKey, JsonWebTokenHandler jsonWebTokenHandler)
    : IJwtGenerator
{
    // private readonly SymmetricSecurityKey _securityKey = securityKey;
    // private readonly JsonWebTokenHandler _jsonWebTokenHandler = jsonWebTokenHandler;

    public string Generate(string userName)
    {
        return string.Empty;
    }
}
