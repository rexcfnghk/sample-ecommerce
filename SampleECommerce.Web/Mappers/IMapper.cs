namespace SampleECommerce.Web.Mappers;

public interface IMapper<TSource, TDest>
{
    TDest Map(TSource source);
}
