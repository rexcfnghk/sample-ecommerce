namespace SampleECommerce.Web.Services;

public interface ISaltService
{
    byte[] GenerateSalt();
}