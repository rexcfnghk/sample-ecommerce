using AutoFixture.Xunit2;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.BasicAuthDecoders;
using SampleECommerce.Web.Dtos;

namespace SampleECommerce.Tests.AuthDecoders;

public class CatchFormatExceptionBasicAuthDecodeTests
{
    [Theory, AutoNSubstituteData]
    public void Decode_ReturnsResultFromDecoratee_IfNoFormatException(
        UserRequestDto expected,
        string base64Encoded,
        [Frozen] IBasicAuthDecoder mockDecoratee,
        CatchFormatExceptionBasicAuthDecoder sut)
    {
        mockDecoratee.Decode(base64Encoded).Returns(expected);

        var actual = sut.Decode(base64Encoded);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory, AutoNSubstituteData]
    public void Decode_ReturnsNull_IfFormatException(
        FormatException e,
        string base64Encoded,
        [Frozen] IBasicAuthDecoder mockDecoratee,
        CatchFormatExceptionBasicAuthDecoder sut)
    {
        mockDecoratee.Decode(base64Encoded).Throws(e);

        var actual = sut.Decode(base64Encoded);
        
        Assert.Null(actual);
    }
}
