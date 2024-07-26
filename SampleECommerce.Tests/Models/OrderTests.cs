using AutoFixture.Idioms;
using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.Models;

namespace SampleECommerce.Tests.Models;

public class OrderTests
{
    [Theory, AutoNSubstituteData]
    public void Constructor_ContainsGuardClauses(GuardClauseAssertion assertion)
    {
        assertion.Verify(typeof(Order).GetConstructors());
    }
    
    [Theory, AutoNSubstituteData]
    public void OrderSum_ReturnsSumOfProductQuantityTimesProductPrice(Order order)
    {
        var expected =
            order.OrderItems.Sum(oi => oi.Quantity * oi.Product.Price);

        var actual = order.OrderSum;
        
        Assert.Equal(expected, actual);
    }
}
