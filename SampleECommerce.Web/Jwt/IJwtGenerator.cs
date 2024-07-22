namespace SampleECommerce.Web.Jwt;

public interface IJwtGenerator
{
    string Generate(int userId);
}
