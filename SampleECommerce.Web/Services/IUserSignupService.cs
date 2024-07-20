using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public interface IUserSignupService
{
    Task<JwtToken> SignupAsync(
        UserSignupRequest request,
        CancellationToken cancellationToken);
}