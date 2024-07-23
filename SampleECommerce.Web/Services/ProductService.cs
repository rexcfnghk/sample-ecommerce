using System.Collections.ObjectModel;
using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;

    public Task<IReadOnlyList<Product>> GetProductsAsync(
        ISet<string> productIds,
        CancellationToken token = default)
    {
        if (productIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<Product>>(
                new ReadOnlyCollection<Product>(new List<Product>()));
        }
        
        return _productRepository.GetProductsAsync(productIds, token);
    }
}
