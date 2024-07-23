using Microsoft.Data.SqlClient;
using SampleECommerce.Web.Exceptions;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Repositories;

public class CatchOrderExceptionOrderRepository(IOrderRepository decoratee) : IOrderRepository
{
    private const int SqlErrorConstraintViolation = 547;
    private readonly IOrderRepository _decoratee = decoratee;

    public Task<IReadOnlyList<Order>> GetOrderItemsAsync(
        int userId,
        CancellationToken token = default)
        => _decoratee.GetOrderItemsAsync(userId, token);

    public async Task PostOrderAsync(int userId, Order order, CancellationToken token = default)
    {
        try
        {
            await _decoratee.PostOrderAsync(
                userId,
                order,
                token);
        }
        catch (OrderException e) when (e.InnerException is SqlException
                                       { Number: SqlErrorConstraintViolation })
        {
            // Handle exception
            // AK_Users_Balance
        }
    }
}
