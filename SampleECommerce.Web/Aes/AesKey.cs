namespace SampleECommerce.Web.Aes;

public sealed class AesKey(byte[] key) : IEquatable<AesKey>
{
    public byte[] Key { get; } = key;

    public bool Equals(AesKey? other)
    {
        if (ReferenceEquals(other, null))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }
        
        return Key.Equals(other.Key);
    }

    public override bool Equals(object? obj) => obj is AesKey other && Equals(other);

    public override int GetHashCode() => Key.GetHashCode();

    public static bool operator ==(AesKey left, AesKey right) => left.Equals(right);

    public static bool operator !=(AesKey left, AesKey right) => !left.Equals(right);
}
