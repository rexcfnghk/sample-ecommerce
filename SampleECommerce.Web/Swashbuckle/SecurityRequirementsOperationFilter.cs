using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SampleECommerce.Web.Swashbuckle;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        var authorizeAttributes = context.MethodInfo.GetCustomAttributes(true)
            .Concat(context.MethodInfo.DeclaringType!.GetCustomAttributes(true))
            .OfType<AuthorizeAttribute>()
            .ToList();
        if (authorizeAttributes.Count == 0)
        {
            return;
        }

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        foreach (var authorizeAttribute in authorizeAttributes)
        {
            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = authorizeAttribute.AuthenticationSchemes
                            }
                        },
                        Array.Empty<string>()
                    }
                });
        }
    }
}
