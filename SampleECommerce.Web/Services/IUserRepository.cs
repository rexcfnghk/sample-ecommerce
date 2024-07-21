using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public interface IUserRepository
{
    Task AddUserAsync(
        string userName,
        byte[] encryptedPassword,
        byte[] salt,
        CancellationToken cancellationToken = default);

    Task<SignedUpUser?> RetrieveUserAsync(
        string userName,
        CancellationToken cancellationToken = default);
}
