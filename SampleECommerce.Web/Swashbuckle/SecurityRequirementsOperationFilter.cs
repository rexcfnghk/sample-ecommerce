using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SampleECommerce.Web.Swashbuckle;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var containsAuthorizeAttribute =
            context.MethodInfo.GetCustomAttributes(true)
                .Any(x => x is AuthorizeAttribute) ||
            (context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .Any(x => x is AuthorizeAttribute) ?? false);
        if (!containsAuthorizeAttribute)
        {
            return;
        }
        
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }, Array.Empty<string>()
                }
            }
        };
    }
}
