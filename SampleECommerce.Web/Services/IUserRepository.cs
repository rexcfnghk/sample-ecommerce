namespace SampleECommerce.Web.Services;

public interface IUserRepository
{
    Task AddUserAsync(
        string userName,
        string encryptedPassword,
        byte[] salt,
        CancellationToken cancellationToken = default);
}