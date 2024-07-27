using SampleECommerce.Web.Dtos;

namespace SampleECommerce.Web.BasicAuthDecoders;

public interface IBasicAuthDecoder
{
    UserRequestDto? Decode(string base64Encoded);
}
