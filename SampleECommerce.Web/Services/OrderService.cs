using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public class OrderService(IOrderRepository orderRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public Task<IReadOnlyList<Order>> ListOrdersAsync(
        int userId,
        CancellationToken token = default) =>
        _orderRepository.GetOrderItemsAsync(userId, token);

    public Task PostOrderAsync(
        int userId,
        Order order,
        CancellationToken token = default) =>
        order.OrderItems.Count == 0
            ? Task.CompletedTask
            : _orderRepository.PostOrderAsync(userId, order, token);
}
