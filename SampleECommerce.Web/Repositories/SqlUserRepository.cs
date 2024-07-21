using Microsoft.Data.SqlClient;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Repositories;

public class SqlUserRepository(string connectionString) : IUserRepository
{
    private const string InsertCommand =
        "INSERT INTO Users (UserName, EncryptedPassword, Salt) VALUES (@UserName, @EncryptedPassword, @Salt)";

    private const string RetrieveQuery =
        "SELECT UserName, EncryptedPassword, Salt FROM Users WHERE UserName = @UserName";
    
    private readonly string _connectionString = connectionString;

    public async Task AddUserAsync(
        string userName,
        byte[] encryptedPassword,
        byte[] salt,
        CancellationToken cancellationToken = default)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(InsertCommand, sqlConnection);
        
        var sqlParameterCollection = command.Parameters;
        sqlParameterCollection.Add(new SqlParameter("@UserName", userName));
        sqlParameterCollection.Add(
            new SqlParameter("@EncryptedPassword", encryptedPassword));
        sqlParameterCollection.Add(new SqlParameter("@Salt", salt));

        await sqlConnection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<SignedUpUser?> RetrieveUserAsync(
        string userName,
        CancellationToken cancellationToken = default)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        await using var query = new SqlCommand(InsertCommand, sqlConnection);
        
        var sqlParameterCollection = query.Parameters;
        sqlParameterCollection.Add(new SqlParameter("@UserName", userName));

        await sqlConnection.OpenAsync(cancellationToken);
        var result = await query.ExecuteReaderAsync(cancellationToken);
        return new SignedUpUser(
            (string)result["UserName"],
            (byte[])result["EncryptedPassword"],
            (byte[])result["Salt"]);
    }
}
