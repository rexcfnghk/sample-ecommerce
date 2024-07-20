using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public class UserSignupService : IUserSignupService
{
    public Task<JwtToken> SignupAsync(
        UserSignupRequest request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}