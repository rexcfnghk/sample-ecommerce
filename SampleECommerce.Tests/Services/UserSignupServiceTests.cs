using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Tests.Services;

public class UserSignupServiceTests
{
    [Theory, AutoNSubstituteData]
    public void UserSignupService_ImplementsIUserSignupService(
        UserSignupService sut)
    {
        Assert.IsAssignableFrom<IUserSignupService>(sut);
    }
}