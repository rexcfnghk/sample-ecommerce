namespace SampleECommerce.Web.Models;

public class SignedUpUser(string userName, byte[] encryptedPassword, byte[] salt) : IEquatable<SignedUpUser>
{
    public string UserName { get; } = userName;

    public byte[] EncryptedPassword { get; } = encryptedPassword;

    public byte[] Salt { get; } = salt;

    public override bool Equals(object? obj)
        => obj is SignedUpUser other && Equals(other);

    public bool Equals(SignedUpUser? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return UserName == other.UserName &&
               EncryptedPassword.SequenceEqual(other.EncryptedPassword) &&
               Salt.SequenceEqual(other.Salt);
    }

    public override int GetHashCode() => HashCode.Combine(UserName, EncryptedPassword, Salt);

    public static bool operator ==(SignedUpUser? left, SignedUpUser? right) => Equals(left, right);

    public static bool operator !=(SignedUpUser? left, SignedUpUser? right) => !Equals(left, right);
}
