using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Repositories;

public class SqlOrderRepository(string connectionString) : IOrderRepository
{
    private const string ProductQuery =
        "SELECT o.Id as OrderId, oi.Id as OrderItemId, p.Name, oi.Quantity, o.OrderTime " +
        "FROM OrderItems oi INNER JOIN dbo.Products p on p.Id = oi.ProductId INNER JOIN Orders o on oi.OrderId = o.Id " +
        "WHERE o.UserId = @UserId";
    
    private readonly string _connectionString = connectionString;

    public async Task<IReadOnlyList<Order>> GetOrderItemsAsync(int userId, CancellationToken token = default)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        await using var query = new SqlCommand(ProductQuery, sqlConnection);
        
        var sqlParameterCollection = query.Parameters;
        sqlParameterCollection.AddWithValue("@UserId", userId);

        await sqlConnection.OpenAsync(token);
        var dataReader = await query.ExecuteReaderAsync(token);

        var orderItems = new List<OrderItem>();
        while (await dataReader.ReadAsync(token))
        {
            var orderItem = new OrderItem(
                dataReader.GetGuid("OrderItemId"),
                dataReader.GetGuid("OrderId"),
                dataReader.GetString("Name"),
                dataReader.GetInt32("Quantity"),
                dataReader.GetDateTimeOffset(dataReader.GetOrdinal("OrderTime")));
            
            orderItems.Add(orderItem);
        }

        var orderLookup = orderItems.ToLookup(
            o => new { o.OrderId, o.OrderTime },
            o => o);

        var orders = from x in orderLookup
            select new Order(x.Key.OrderId, x.Key.OrderTime, x.ToList());

        return orders.ToList();
    }
}
