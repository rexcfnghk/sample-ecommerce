using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SampleECommerce.Web.Exceptions;

namespace SampleECommerce.Web.Filters;

[UsedImplicitly]
public class HandleDuplicateUserNameExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not DuplicateUserNameException ex)
        {
            return;
        }

        context.Result =
            new BadRequestObjectResult($"The username '{ex.DuplicateUserName}' is already taken.");
    }
}