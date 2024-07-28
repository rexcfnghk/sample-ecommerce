using AutoFixture.Xunit2;
using SampleECommerce.Web.Aes;

namespace SampleECommerce.Tests.Models;

public class AesKeyTests
{
    [Theory, AutoData]
    public void
        GetHashCode_ShouldReturnEqualValues_WhenTwoInstancesHaveTheEqualUnderlyingByteArrays(
            [Frozen] byte[] source,
            AesKey sut1)
    {
        // Arrange
        var copy = new byte[source.Length];
        Array.Copy(source, copy, source.Length);
        var sut2 = new AesKey(copy);
        var expected = sut1.GetHashCode();
        
        // Act
        var actual = sut2.GetHashCode();
        
        Assert.Equal(expected, actual);
    }
}
