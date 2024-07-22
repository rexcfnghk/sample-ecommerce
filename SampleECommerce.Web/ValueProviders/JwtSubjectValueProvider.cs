using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SampleECommerce.Web.ValueProviders;

public class JwtSubjectValueProvider(
    JsonWebToken jsonWebToken)
    : IValueProvider
{
    private readonly JsonWebToken _jsonWebToken = jsonWebToken;

    public bool ContainsPrefix(string prefix)
    {
        return true;
    }

    public ValueProviderResult GetValue(string key)
    {
        var subject = _jsonWebToken.Subject;
        if (subject == null)
        {
            throw new InvalidOperationException(
                "Unable to retrieve subject from JWT");
        }

        return new ValueProviderResult(new StringValues(subject));
    }
}
