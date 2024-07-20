namespace SampleECommerce.Web.Services;

public interface IUserRepository
{
    Task AddUserAsync(
        string userName,
        byte[] encryptedPassword,
        byte[] salt,
        CancellationToken cancellationToken = default);
}