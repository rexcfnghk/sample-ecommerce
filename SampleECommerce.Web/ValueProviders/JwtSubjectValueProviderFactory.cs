using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleInjector;

namespace SampleECommerce.Web.ValueProviders;

public class JwtSubjectValueProviderFactory : IValueProviderFactory
{
    public Task CreateValueProviderAsync(
        ValueProviderFactoryContext context)
    {
        var authorization =
            context.ActionContext.HttpContext.Request.Headers
                .Authorization.FirstOrDefault();

        if (authorization == null)
        {
            return Task.CompletedTask;
        }

        // Remove the word 'Bearer'
        var trimmed = authorization[7..];
        var jsonWebTokenHandler = context.ActionContext.HttpContext
            .RequestServices.GetRequiredService<JsonWebTokenHandler>();
        var token = jsonWebTokenHandler.ReadJsonWebToken(trimmed);
        if (token == null)
        {
            return Task.CompletedTask;
        }

        var valueProvider = new JwtSubjectValueProvider(token);
        context.ValueProviders.Insert(0, valueProvider);
        return Task.CompletedTask;
    }
}
