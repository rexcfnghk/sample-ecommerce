using AutoFixture.Xunit2;
using SampleECommerce.Web.Models;

namespace SampleECommerce.Tests.Models;

public class UserIdTests
{
    [Theory, AutoData]
    public void GetHashCode_ReturnsUnderlyingInteger(
        [Frozen] int expected,
        UserId sut)
    {
        var actual = sut.GetHashCode();
        
        Assert.Equal(expected, actual);
    }

    [Theory, AutoData]
    public void Equals_ReturnsTrue_WhenTwoInstancesHaveTheSameUnderlyingInteger(
        [Frozen] int _,
        UserId x,
        UserId y)
    {
        Assert.Equal(x, y);
    }

    [Theory, AutoData]
    public void
        Equals_ReturnsFalse_WhenTwoInstancesDoNotHaveTheSameUnderlyingInteger(
            [Frozen] int userId,
            UserId x)
    {
        var y = new UserId(userId + 1);
        
        Assert.NotEqual(x, y);
    }

    [Theory, AutoData]
    public void ImplicitOperator_ReturnsUnderlyingInteger(
        [Frozen] int expected,
        UserId sut)
    {
        int actual = sut;
        
        Assert.Equal(expected, actual);
    }

    [Theory, AutoData]
    public void ExplicitOeprator_ReturnsInstanceWithEqualUnderlyingInteger(
        int expected)
    {
        var actual = (UserId)expected;
        
        Assert.Equal(expected, actual);
    }
}
