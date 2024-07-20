namespace SampleECommerce.Web.Services;

public interface IPasswordEncryptionService
{
    byte[] Encrypt(string password, byte[] salt);
}