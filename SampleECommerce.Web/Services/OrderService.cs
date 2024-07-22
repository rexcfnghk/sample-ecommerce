using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public class OrderService(IOrderRepository orderRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public Task<IReadOnlyList<Order>> ListOrdersAsync(int userId, CancellationToken token = default)
    {
        return _orderRepository.GetOrderItemsAsync(userId, token);
    }
}
