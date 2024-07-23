using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetProductsAsync(
        ISet<string> productIds,
        CancellationToken token = default);
}
