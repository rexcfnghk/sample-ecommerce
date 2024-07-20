using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Services;

public class UserSignupService : IUserSignupService
{
    private readonly ISaltService _saltService;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly IUserRepository _userRepository;

    public UserSignupService(
        ISaltService saltService,
        IPasswordEncryptionService passwordEncryptionService,
        IUserRepository userRepository)
    {
        _saltService = saltService;
        _passwordEncryptionService = passwordEncryptionService;
        _userRepository = userRepository;
    }

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