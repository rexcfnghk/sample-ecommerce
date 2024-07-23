using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SampleECommerce.Web.Exceptions;
using SampleECommerce.Web.Repositories;

namespace SampleECommerce.Web.Filters;

public class HandleOrderExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        if (exception is not OrderException orderException)
        {
            return;
        }

        var errorType = orderException.OrderErrorType;
        switch (errorType)
        {
            case OrderErrorType.OrderItemQuantityEqualToOrLessThanZero:
                context.Result = new BadRequestObjectResult(
                    "One of the order items has an order quantity equal to less than zero");
                return;
            case OrderErrorType.ProductQuantityLessThanOrderQuantity:
                context.Result = new BadRequestObjectResult(
                    "One of the order items has been over-ordered");
                return;
            case OrderErrorType.UserBalanceLessThanSumOfOrder:
                context.Result =
                    new BadRequestObjectResult("User balance is insufficient");
                return;
            case OrderErrorType.None:
            default:
                return;
        }
    }
}
