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
        IReadOnlyList<Order> expected,
        [Frozen] IOrderRepository mockOrderRepository,
        OrderService sut,
        CancellationToken token)
    {
        mockOrderRepository.GetOrderItemsAsync(userId, token)
            .Returns(Task.FromResult(expected));

        var actual = await sut.ListOrdersAsync(userId, token);
        
        Assert.Equivalent(expected, actual);
    }
    
    [Theory, AutoNSubstituteData]
    public async Task PostOrderAsync_DoesNotCallOrderRepository(
        int userId,
        Order order,
        [Frozen] IOrderRepository mockOrderRepository,
        OrderService sut,
        CancellationToken token)
    {
        mockOrderRepository.PostOrderAsync(userId, order, token)
            .Returns(Task.CompletedTask);

        await sut.PostOrderAsync(userId, order, token);

        mockOrderRepository.Received()
            .PostOrderAsync(userId, order, token);
    }
    
    [Theory, AutoNSubstituteData]
    public void GenerateOrder_ReturnsExpectedOrder(
        Guid orderId,
        DateTimeOffset orderTime,
        IReadOnlyList<OrderItem> orderItems,
    [Frozen] IOrderIdGenerator mockOrderIdGenerator,
        [Frozen] IOrderTimeGenerator mockOrderItemGenerator,
        OrderService sut,
        CancellationToken token)
    {
        mockOrderIdGenerator.GenerateOrderId().Returns(orderId);
        mockOrderItemGenerator.GenerateOrderTime().Returns(orderTime);

        var order = sut.GenerateOrder(orderItems);

        Assert.Equal(orderId, order.Id);
        Assert.Equal(orderTime, order.OrderTime);
    }
}
