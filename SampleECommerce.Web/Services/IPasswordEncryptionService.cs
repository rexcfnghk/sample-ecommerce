namespace SampleECommerce.Web.Services;

public interface IPasswordEncryptionService
{
    string Encrypt(string password, byte[] salt);
}