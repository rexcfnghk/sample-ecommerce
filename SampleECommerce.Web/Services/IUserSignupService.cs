using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public interface IUserSignupService
{
    Task SignupAsync(
        UserSignupRequest request,
        CancellationToken cancellationToken);
}