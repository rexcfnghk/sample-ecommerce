using System.Data;
using Microsoft.Data.SqlClient;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Repositories;

public class SqlOrderRepository(string connectionString) : IOrderRepository
{
    private const string OrderQuery =
        "SELECT o.Id as OrderId, oi.Id as OrderItemId, p.Id as ProductId, " + 
            "p.Name as ProductName, p.Quantity as ProductQuantity, p.Price as ProductPrice, p.Category as ProductCategory, " +
            "oi.Quantity as OrderQuantity, o.OrderTime " +
        "FROM OrderItems oi INNER JOIN dbo.Products p on p.Id = oi.ProductId INNER JOIN Orders o on oi.OrderId = o.Id " +
        "WHERE o.UserId = @UserId";

    private const string InsertOrderCommand =
        "INSERT INTO Orders (Id, UserId, OrderTime) VALUES (@Id, @UserId, @OrderTime)";

    private const string UpdateBalanceCommand =
        "UPDATE Users SET Balance = Balance - @OrderTotal WHERE Id = @UserId";

    private const string UpdateProductCommand =
        "UPDATE Products SET Quantity = Quantity - @OrderQuantity WHERE Id = @ProductId";

    private const string InsertOrderItemCommand =
        "INSERT INTO OrderItems (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)";
    
    private readonly string _connectionString = connectionString;

    public async Task<IReadOnlyList<Order>> GetOrderItemsAsync(int userId, CancellationToken token = default)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        await using var query = sqlConnection.CreateCommand();
        
        query.CommandText = OrderQuery;
        
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
                dataReader.GetString("ProductCategory"),
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
                                oi.ProductPrice,
                                oi.ProductCategory),
                            oi.OrderQuantity))
                    .ToList());

        return orders.ToList();
    }

    public async Task PostOrderAsync(int userId, Order order, CancellationToken token = default)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        
        await sqlConnection.OpenAsync(token);
        
        await using var command = sqlConnection.CreateCommand();
        
        var transaction = (SqlTransaction)await sqlConnection.BeginTransactionAsync(token);

        command.CommandText = InsertOrderCommand;
        command.Transaction = transaction;
        command.Parameters.AddWithValue("@Id", order.Id);
        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@OrderTime", order.OrderTime);

        await command.ExecuteNonQueryAsync(token);

        command.Parameters.Clear();
        command.CommandText = UpdateBalanceCommand;
        
        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@OrderTotal", order.OrderSum);
        
        await command.ExecuteNonQueryAsync(token);

        foreach (var orderItem in order.OrderItems)
        {
            command.Parameters.Clear();
            command.CommandText = UpdateProductCommand;

            command.Parameters.AddWithValue("@ProductId", orderItem.Product.Id);
            command.Parameters.AddWithValue(
                "@OrderQuantity",
                orderItem.Quantity);

            await command.ExecuteNonQueryAsync(token);
            
            command.Parameters.Clear();
            command.CommandText = InsertOrderItemCommand;

            command.Parameters.AddWithValue("@OrderId", order.Id);
            command.Parameters.AddWithValue("@ProductId", orderItem.Product.Id);
            command.Parameters.AddWithValue("@Quantity", orderItem.Quantity);
            
            await command.ExecuteNonQueryAsync(token);

        }

        try
        {
            await transaction.CommitAsync(token);
        }
        catch (Exception)
        {
            try
            {
                await transaction.RollbackAsync(token);
            }
            catch (Exception inner)
            {
                throw new InvalidOperationException("Rollback failed", inner);
            }
        }

    }
}
