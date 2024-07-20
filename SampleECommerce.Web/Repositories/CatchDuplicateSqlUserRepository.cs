using Microsoft.Data.SqlClient;
using SampleECommerce.Web.Exceptions;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Repositories;

public class CatchDuplicateSqlUserRepository : IUserRepository
{
    private const int UniqueConstraintViolationErrorNumber = 2627;
    private readonly IUserRepository _decoratee;

    public CatchDuplicateSqlUserRepository(IUserRepository decoratee)
    {
        _decoratee = decoratee;
    }
    public async Task AddUserAsync(
        string userName,
        byte[] encryptedPassword,
        byte[] salt,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _decoratee.AddUserAsync(
                userName,
                encryptedPassword,
                salt,
                cancellationToken);
        }
        catch (SqlException e) when (e.Number == UniqueConstraintViolationErrorNumber)
        {
            throw new DuplicateUserNameException(
                userName,
                $@"A user with the username {userName} already exists.",
                e);
        }
    }
}