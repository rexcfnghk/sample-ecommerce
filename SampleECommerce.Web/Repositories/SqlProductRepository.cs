using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Repositories;

public class SqlProductRepository(string connectionString) : IProductRepository
{
    private readonly string _connectionString = connectionString;

    private const string ProductQuery =
        "SELECT Id, Name, Price, Quantity, Category FROM Products WHERE Id in (@Ids)";

    
    public async Task<IReadOnlyList<Product>> GetProductsAsync(ISet<string> productIds, CancellationToken token = default)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        await using var command = sqlConnection.CreateCommand();

        command.CommandText = ProductQuery;
        command.Parameters.AddWithValue("@Ids", string.Join(',', productIds));

        await sqlConnection.OpenAsync(token);

        var reader = await command.ExecuteReaderAsync(token);

        var result = new List<Product>();

        while (await reader.ReadAsync(token))
        {
            var product = new Product(
                reader.GetString("Id"),
                reader.GetString("Name"),
                reader.GetInt32("Quantity"),
                reader.GetDecimal("Price"),
                reader.GetString("Category"));
            
            result.Add(product);
        }

        return new ReadOnlyCollection<Product>(result);
    }
}
