using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public interface IOrderService
{
    Task<IReadOnlyList<Order>> ListOrdersAsync(
        int userId,
        CancellationToken token = default);
}
