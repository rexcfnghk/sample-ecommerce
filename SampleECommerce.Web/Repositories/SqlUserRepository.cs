using System.Data;
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
        sqlParameterCollection.AddWithValue("@UserName", userName);
        sqlParameterCollection.AddWithValue("@EncryptedPassword", encryptedPassword);
        sqlParameterCollection.AddWithValue("@Salt", salt);

        await sqlConnection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<SignedUpUser?> RetrieveUserAsync(
        string userName,
        CancellationToken cancellationToken = default)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        await using var query = new SqlCommand(RetrieveQuery, sqlConnection);
        
        var sqlParameterCollection = query.Parameters;
        sqlParameterCollection.AddWithValue("@UserName", userName);

        await sqlConnection.OpenAsync(cancellationToken);
        var result = await query.ExecuteReaderAsync(cancellationToken);
        var hasRows = await result.ReadAsync(cancellationToken);
        if (!hasRows)
        {
            return null;
        }

        var encryptedPassword = new byte[16];
        result.GetBytes(
            "EncryptedPassword",
            0L,
            encryptedPassword,
            0,
            encryptedPassword.Length);
        
        var salt = new byte[16];
        result.GetBytes(
            "Salt",
            0L,
            salt,
            0,
            salt.Length);

        return new SignedUpUser(
            result.GetString("UserName"),
            encryptedPassword,
            salt);
    }
}
