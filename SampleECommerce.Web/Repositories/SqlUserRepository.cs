using Microsoft.Data.SqlClient;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Repositories;

public class SqlUserRepository : IUserRepository
{
    private static readonly string _command =
        "INSERT INTO Users (UserName, EncryptedPassword, Salt) VALUES (@UserName, @EncryptedPassword, @Salt)";
    
    private readonly string _connectionString;

    public SqlUserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task AddUserAsync(
        string userName,
        byte[] encryptedPassword,
        byte[] salt,
        CancellationToken cancellationToken = default)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(_command, sqlConnection);
        
        var sqlParameterCollection = command.Parameters;
        sqlParameterCollection.Add(new SqlParameter("@UserName", userName));
        sqlParameterCollection.Add(
            new SqlParameter("@EncryptedPassword", encryptedPassword));
        sqlParameterCollection.Add(new SqlParameter("@Salt", salt));

        await sqlConnection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}