using SampleECommerce.Web.Dtos;

namespace SampleECommerce.Web.BasicAuthDecoders;

public class CatchFormatExceptionBasicAuthDecoder(IBasicAuthDecoder decoratee) : IBasicAuthDecoder
{
    private readonly IBasicAuthDecoder _decoratee = decoratee;
    
    public UserRequestDto? Decode(string base64Encoded)
    {
        try
        {
            return _decoratee.Decode(base64Encoded);
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
