using AutoFixture.Xunit2;
using NSubstitute;
using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Tests.Services;

public class UserSignupServiceTests
{
    [Theory, AutoNSubstituteData]
    public void UserSignupService_ImplementsIUserSignupService(
        UserSignupService sut)
    {
        Assert.IsAssignableFrom<IUserSignupService>(sut);
    }

    [Theory, AutoNSubstituteData]
    public async Task SignupAsync_SavesUserWithEncryptedPassword(
        byte[] salt,
        byte[] encryptedPassword,
        UserSignupRequest request,
        [Frozen] ISaltService saltService,
        [Frozen] IPasswordEncryptionService passwordEncryptionService,
        [Frozen] IUserRepository userRepository,
        UserSignupService sut,
        CancellationToken token)
    {
        // Arrange
        saltService.GenerateSalt().Returns(salt);
        passwordEncryptionService.Encrypt(request.Password, salt).Returns(encryptedPassword);
        userRepository.AddUserAsync(request.Username, encryptedPassword, salt, token)
            .Returns(Task.CompletedTask);
        
        // Act
        await sut.SignupAsync(request, token);
        
        // Assert
        userRepository.Received()
            .AddUserAsync(request.Username, encryptedPassword, salt, token);
    }
}