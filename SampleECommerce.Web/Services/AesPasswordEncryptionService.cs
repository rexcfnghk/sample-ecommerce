using System.Security.Cryptography;
using SampleECommerce.Web.Aes;

namespace SampleECommerce.Web.Services;

public class AesPasswordEncryptionService(AesKey aesKey) : IPasswordEncryptionService
{
    // Need to make sure aes.Key is identical between persistence
    private readonly AesKey _aesKey = aesKey;
    
    public byte[] Encrypt(string password, byte[] salt)
    {
        using var aes = System.Security.Cryptography.Aes.Create();
        var encrypted = EncryptWithAes(password, _aesKey.Key, salt);
        return encrypted;
    }

    private static byte[] EncryptWithAes(
        string plainText,
        byte[] key,
        byte[] iv)
    {
        using var aes = System.Security.Cryptography.Aes.Create();
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
