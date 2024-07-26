using AutoFixture.Idioms;
using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.Models;

namespace SampleECommerce.Tests.Models;

public class ProductTests
{
    [Theory, AutoNSubstituteData]
    public void Constructor_ContainsGuardClauses(GuardClauseAssertion assertion)
    {
        assertion.Verify(typeof(Product).GetConstructors());
    }
}
