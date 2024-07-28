namespace SampleECommerce.Web.Repositories;

public sealed class ConnectionString(string connectionString) : IEquatable<ConnectionString>
{
    private readonly string _connectionString = connectionString;

    public static implicit operator string(ConnectionString s) =>
        s._connectionString;

    public static explicit operator ConnectionString(string s)
        => new(s);

    public override string ToString() => _connectionString;

    public override bool Equals(object? obj)
        => obj is ConnectionString other && Equals(other);

    public bool Equals(ConnectionString? other)
        => other is not null && (ReferenceEquals(this, other) || string.Equals(
            _connectionString,
            other._connectionString,
            StringComparison.Ordinal));

    public override int GetHashCode()
        => _connectionString.GetHashCode();
}
