using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetOrderItemsAsync(
        int userId,
        CancellationToken token = default);
}
