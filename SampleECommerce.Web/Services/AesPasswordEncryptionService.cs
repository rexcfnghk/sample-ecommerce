using System.Security.Cryptography;
using System.Text;

namespace SampleECommerce.Web.Services;

public class AesPasswordEncryptionService : IPasswordEncryptionService
{
    public byte[] Encrypt(string password, byte[] salt)
    {
        using var aes = Aes.Create();
        var encrypted = EncryptWithAes(password, aes.Key, salt);
        return encrypted;
    }

    private static byte[] EncryptWithAes(
        string plainText,
        byte[] key,
        byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(
            memoryStream,
            encryptor,
            CryptoStreamMode.Write);
        using (var streamWriter = new StreamWriter(cryptoStream))
            streamWriter.Write(plainText);
        
        return memoryStream.ToArray();
    }
}