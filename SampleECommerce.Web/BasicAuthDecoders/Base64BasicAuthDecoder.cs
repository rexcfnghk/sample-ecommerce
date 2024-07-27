using SampleECommerce.Web.Dtos;
using System.Text;

namespace SampleECommerce.Web.BasicAuthDecoders;

public class Base64BasicAuthDecoder : IBasicAuthDecoder
{
    public UserRequestDto? Decode(string base64Encoded)
    {
        var data = Convert.FromBase64String(base64Encoded);
        var decoded = Encoding.UTF8.GetString(data);
        var split = decoded.Split(':');
        return split.Length != 2
            ? null
            : new UserRequestDto { UserName = split[0], Password = split[1] };
    }
}
