using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SampleECommerce.Web.Swashbuckle;

public class OpenApiParameterIgnoreFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription?.ParameterDescriptions == null)
        {
            return;
        }

        var parametersToHide = context.ApiDescription.ParameterDescriptions
            .Where(ParameterHasIgnoreAttribute)
            .ToList();

        if (parametersToHide.Count == 0)
        {
            return;
        }

        foreach (var parameter in parametersToHide.Select(
                         parameterToHide => operation.Parameters.FirstOrDefault(
                             parameter => string.Equals(
                                 parameter.Name,
                                 parameterToHide.Name,
                                 StringComparison.Ordinal)))
                     .OfType<OpenApiParameter>())
        {
            operation.Parameters.Remove(parameter);
        }
    }

    private static bool ParameterHasIgnoreAttribute(Microsoft.AspNetCore.Mvc.ApiExplorer.ApiParameterDescription parameterDescription)
    {
        if (parameterDescription.ModelMetadata is Microsoft.AspNetCore.Mvc.
            ModelBinding.Metadata.DefaultModelMetadata metadata)
        {
            return metadata.Attributes.ParameterAttributes != null &&
                   metadata.Attributes.ParameterAttributes.Any(
                       attribute =>
                           attribute is OpenApiParameterIgnoreAttribute);
        }

        return false;
    }
}
