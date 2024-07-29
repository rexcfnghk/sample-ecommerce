namespace SampleECommerce.Web.Models;

public readonly struct UserId(int userId) : IEquatable<UserId>
{
    private readonly int _userId = userId;

    public static explicit operator UserId(int userId) => new(userId);

    public static implicit operator int(UserId userId) => userId._userId;

    public bool Equals(UserId other) => _userId == other._userId;

    public override bool Equals(object? obj) => obj is UserId other && Equals(other);

    public override int GetHashCode() => _userId;

    public static bool operator ==(UserId left, UserId right) => left.Equals(right);

    public static bool operator !=(UserId left, UserId right) => !left.Equals(right);
}
