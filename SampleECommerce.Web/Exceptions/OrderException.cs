using JetBrains.Annotations;
using SampleECommerce.Web.Repositories;

namespace SampleECommerce.Web.Exceptions;

public class OrderException : Exception
{
    [PublicAPI]
    public OrderException()
    {
    }

    public OrderException(string message) : base(message)
    {
        
    }

    public OrderException(OrderErrorType orderErrorType, string message, Exception innerException) : base(
        message,
        innerException)
    {
        OrderErrorType = orderErrorType;
    }
    
    public OrderErrorType OrderErrorType { get; }
}
