namespace SampleECommerce.Web.Jwt;

public interface IJwtGenerator
{
    string Generate(string userName);
}
