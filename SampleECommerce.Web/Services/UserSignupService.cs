using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public class UserSignupService(
    ISaltService saltService,
    IPasswordEncryptionService passwordEncryptionService,
    IUserRepository userRepository)
    : IUserSignupService
{
    private readonly ISaltService _saltService = saltService;

    private readonly IPasswordEncryptionService _passwordEncryptionService =
        passwordEncryptionService;

    private readonly IUserRepository _userRepository = userRepository;

    public Task SignupAsync(
        UserSignupRequest request,
        CancellationToken cancellationToken = default)
    {
        var salt = _saltService.GenerateSalt();

        var encryptedPassword =
            _passwordEncryptionService.Encrypt(request.Password, salt);

        return _userRepository.AddUserAsync(
            request.Username,
            encryptedPassword,
            salt,
            cancellationToken);
    }
}