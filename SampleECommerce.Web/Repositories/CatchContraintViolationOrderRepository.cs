using Microsoft.Data.SqlClient;
using SampleECommerce.Web.Exceptions;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Repositories;

public class CatchContraintViolationOrderRepository(IOrderRepository decoratee) : IOrderRepository
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
        catch (SqlException e) when (e.Number == SqlErrorConstraintViolation)
        {
            var orderErrorType = GetOrderErrorType(e);
            throw new OrderException(
                orderErrorType,
                "Error occurred when placing an order",
                e);
        }
    }

    private static OrderErrorType GetOrderErrorType(SqlException e) =>
        e.Message switch
        {
            { } m when m.Contains("AK_Orders_Quantity") => OrderErrorType
                .OrderItemQuantityEqualToOrLessThanZero,
            { } m when m.Contains("AK_Products_Quantity") => OrderErrorType
                .ProductQuantityLessThanOrderQuantity,
            { } m when m.Contains("AK_Users_Balance") => OrderErrorType
                .UserBalanceLessThanSumOfOrder,
            _ => OrderErrorType.None
        };
}
