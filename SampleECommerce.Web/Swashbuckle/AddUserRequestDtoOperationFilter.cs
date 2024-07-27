using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using SampleECommerce.Web.Constants;
using SampleECommerce.Web.Dtos;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SampleECommerce.Web.Swashbuckle;

public class AddUserRequestDtoOperationFilter : IOperationFilter
{
    // Retrospectively add username and password into request body of actions 
    // using username and password authentication
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes =
            from attribute in context.ApiDescription.CustomAttributes()
                .OfType<AuthorizeAttribute>()
            where attribute.AuthenticationSchemes ==
                  AuthenticationSchemes.UserNamePassword
            select attribute;

        foreach (var _ in attributes)
        {
            operation.RequestBody ??= new OpenApiRequestBody
            {
                Required = true,
                Description = "Username and password",
            };
            operation.RequestBody.Content.Add(
                MediaTypeNames.Application.Json,
                new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = $"{nameof(UserRequestDto)}"
                        }
                    }
                });
        }
    }
}
