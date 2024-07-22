using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Repositories;

public class SqlOrderRepository(string connectionString) : IOrderRepository
{
    private const string ProductQuery =
        "SELECT o.Id, p.Name, o.Quantity, OrderTime " +
        "FROM Orders o INNER JOIN dbo.Products p on p.Id = o.ProductId " +
        "WHERE UserId = @UserId";
    
    private readonly string _connectionString = connectionString;

    public async Task<IReadOnlyList<Order>> GetOrdersAsync(int userId, CancellationToken token = default)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        await using var query = new SqlCommand(ProductQuery, sqlConnection);
        
        var sqlParameterCollection = query.Parameters;
        sqlParameterCollection.AddWithValue("@UserId", userId);

        await sqlConnection.OpenAsync(token);
        var dataReader = await query.ExecuteReaderAsync(token);

        var result = new List<Order>();
        while (await dataReader.ReadAsync(token))
        {
            var order = new Order(
                dataReader.GetGuid("Id"),
                dataReader.GetString("Name"),
                dataReader.GetInt32("Quantity"),
                dataReader.GetDateTimeOffset(dataReader.GetOrdinal("OrderTime")));
            
            result.Add(order);
        }

        return new ReadOnlyCollection<Order>(result);
    }
}
