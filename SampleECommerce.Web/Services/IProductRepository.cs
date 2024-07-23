using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetProductsAsync(
        ISet<string> productIds,
        CancellationToken token = default);
}
