using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Repositories;

public class SqlOrderRepository(string connectionString) : IOrderRepository
{
    private const string ProductQuery =
        "SELECT o.Id as OrderId, oi.Id as OrderItemId, p.Id as ProductId, p.Name as ProductName, p.Quantity as ProductQuantity, p.Price as ProductPrice, oi.Quantity as OrderQuantity, o.OrderTime " +
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

        var orderItems = new List<OrderItemRow>();
        while (await dataReader.ReadAsync(token))
        {
            var orderItem = new OrderItemRow(
                dataReader.GetGuid("OrderItemId"),
                dataReader.GetGuid("OrderId"),
                dataReader.GetString("ProductId"),
                dataReader.GetString("ProductName"),
                dataReader.GetInt32("ProductQuantity"),
                dataReader.GetDecimal("ProductPrice"),
                dataReader.GetInt32("OrderQuantity"),
                dataReader.GetDateTimeOffset(dataReader.GetOrdinal("OrderTime")));
            
            orderItems.Add(orderItem);
        }

        var orderLookup = orderItems.ToLookup(
            o => new { o.OrderId, o.OrderTime },
            o => o);

        var orders = from x in orderLookup
            select new Order(
                x.Key.OrderId,
                x.Key.OrderTime,
                x.Select(
                        oi => new OrderItem(
                            new Product(
                                oi.ProductId,
                                oi.ProductName,
                                oi.ProductQuantity,
                                oi.ProductPrice),
                            oi.OrderQuantity))
                    .ToList());

        return orders.ToList();
    }
}
