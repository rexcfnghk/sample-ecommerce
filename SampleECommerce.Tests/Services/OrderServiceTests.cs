using AutoFixture.Xunit2;
using NSubstitute;
using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Tests.Services;

public class OrderServiceTests
{
    [Theory, AutoNSubstituteData]
    public void IsAssignableFromIOrderService(OrderService sut)
    {
        Assert.IsAssignableFrom<IOrderService>(sut);
    }

    [Theory, AutoNSubstituteData]
    public async Task ListOrderAsync_ReturnsExpectedGrouping(
        int userId,
        IReadOnlyList<OrderItem> orders,
        [Frozen] IOrderRepository mockOrderRepository,
        OrderService sut,
        CancellationToken token)
    {
        mockOrderRepository.GetOrderItemsAsync(userId, token)
            .Returns(Task.FromResult(orders));
        var expected = from order in orders group order by order.Id;

        var actual = await sut.ListOrdersAsync(userId, token);
        
        Assert.Equivalent(expected, actual);
    }
}
