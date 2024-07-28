using AutoFixture.Xunit2;
using SampleECommerce.Web.Repositories;

namespace SampleECommerce.Tests.Models;

public class ConnectionStringTests
{
    [Theory, AutoData]
    public void Equals_ReturnTrue_WhenTwoInstancesHaveTheSameStringContent(
        [Frozen] string _,
        ConnectionString c1,
        ConnectionString c2)
    {
        Assert.Equal(c1, c2);
    }

    [Theory, AutoData]
    public void
        Equals_ReturnsFalse_WhenTwoInstancesDoNotHaveTheSameStringContent(
            string s2,
            [Frozen] string s1,
            ConnectionString c1)
    {
        var newConnectionString = s1 + s2;
        var c2 = new ConnectionString(newConnectionString);
        
        Assert.NotEqual(c1, c2);
    }

    [Theory, AutoData]
    public void
        GetHashCode_ReturnsSameValue_WhenTwoInstancesHaveTheSameStringContent(
            [Frozen] string _,
            ConnectionString c1,
            ConnectionString c2)
    {
        Assert.Equal(c1.GetHashCode(), c2.GetHashCode());
    }

    [Theory, AutoData]
    public void GetHashCode_ReturnsTheSameHashCodeAsUnderlyingString(
        [Frozen] string s,
        ConnectionString sut)
    {
        var expected = s.GetHashCode();

        var actual = sut.GetHashCode();
        
        Assert.Equal(expected, actual);
    }

    [Theory, AutoData]
    public void ToString_ReturnsUnderlyingString(
        [Frozen] string expected,
        ConnectionString sut)
    {
        var actual = sut.ToString();
        
        Assert.Equal(expected, actual);
    }

    [Theory, AutoData]
    public void ImplicitOperator_ReturnsUnderlyingString(
        [Frozen] string expected,
        ConnectionString actual)
    {
        Assert.Equal(expected, actual);
    }
    
    [Theory, AutoData]
    public void ExplicitOperator_ReturnsInstanceWithSameUnderlyingString(
        string expected)
    {
        var actual = (ConnectionString)expected;
        
        Assert.Equal(expected, actual);
    }
}
