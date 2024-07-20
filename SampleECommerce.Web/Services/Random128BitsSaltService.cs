using System.Security.Cryptography;

namespace SampleECommerce.Web.Services;

public class Random128BitsSaltService : ISaltService
{
    public byte[] GenerateSalt()
        => RandomNumberGenerator.GetBytes(16);
}