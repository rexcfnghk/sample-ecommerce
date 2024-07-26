using AutoFixture.Xunit2;
using NSubstitute;
using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Tests.Services;

public class ProductServiceTests
{
    [Theory, AutoNSubstituteData]
    public void IsAssignableFromIProductService(ProductService sut)
    {
        Assert.IsAssignableFrom<IProductService>(sut);
    }
    
    [Theory, AutoNSubstituteData]
    public async Task
        GetProductAsync_ReturnsEmptyReadOnlyCollection_WhenProductIdsIsEmpty(
            ProductService sut,
            CancellationToken cancellationToken)
    {
        var productIds = new HashSet<string>();

        var actual = await sut.GetProductsAsync(productIds, cancellationToken);
        
        Assert.Empty(actual);
    }

    [Theory, AutoNSubstituteData]
    public async Task
        GetProductAsync_ReturnsCollectionFromRepository_WhenProductIdsIsNotEmpty(
            ISet<string> productIds,
            IReadOnlyList<Product> expected,
            [Frozen] IProductRepository mockProductRepository,
            ProductService sut,
            CancellationToken cancellationToken)
    {
        mockProductRepository.GetProductsAsync(productIds, cancellationToken)
            .Returns(expected);

        var actual = await sut.GetProductsAsync(productIds, cancellationToken);

        Assert.Equal(expected, actual);
    }
}
