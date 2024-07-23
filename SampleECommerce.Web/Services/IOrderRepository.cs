using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetOrderItemsAsync(
        int userId,
        CancellationToken token = default);

    Task PostOrderAsync(
        int userId,
        Order order,
        CancellationToken token = default);
}
