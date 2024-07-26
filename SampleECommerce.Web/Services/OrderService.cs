using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public class OrderService(IOrderRepository orderRepository, IOrderIdGenerator orderIdGenerator, IOrderTimeGenerator orderTimeGenerator) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IOrderIdGenerator _orderIdGenerator = orderIdGenerator;

    private readonly IOrderTimeGenerator _orderTimeGenerator =
        orderTimeGenerator;

    public Task<IReadOnlyList<Order>> ListOrdersAsync(
        int userId,
        CancellationToken token = default) =>
        _orderRepository.GetOrderItemsAsync(userId, token);

    public Task PostOrderAsync(
        int userId,
        Order order,
        CancellationToken token = default) =>
        _orderRepository.PostOrderAsync(userId, order, token);

    public Order GenerateOrder(IReadOnlyList<OrderItem> orderItems)
    {
        var orderId = _orderIdGenerator.GenerateOrderId();
        var orderTime = _orderTimeGenerator.GenerateOrderTime();
        return new Order(orderId, orderTime, orderItems);
    }
}
