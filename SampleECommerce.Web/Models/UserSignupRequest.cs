namespace SampleECommerce.Web.Models;

public class UserSignupRequest(string username, string password)
{
    public string Username { get; } = username;

    public string Password { get; } = password;
}