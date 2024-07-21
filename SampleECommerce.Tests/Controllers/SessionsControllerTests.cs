using System.Text;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.Controllers;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Jwt;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Tests.Controllers;

public class SessionsControllerTests
{
    [Theory, AutoNSubstituteData]
    public void IsAssignableFromControllerBase(SessionsController sut)
    {
        Assert.IsAssignableFrom<ControllerBase>(sut);
    }

    [Theory, AutoNSubstituteData]
    public async Task CreateSession_GeneratesExpectedJwt(
        string expected,
        UserRequestDto userRequestDto,
        SignedUpUser signedUpUser,
        [Frozen] IUserRepository userRepository,
        [Frozen] IPasswordEncryptionService passwordEncryptionService,
        [Frozen] IJwtGenerator jwtGenerator,
        SessionsController sut,
        CancellationToken token)
    {
        // Arrange
        userRepository.RetrieveUserAsync(userRequestDto.UserName, token)
            .Returns(Task.FromResult(signedUpUser));
        
        passwordEncryptionService
            .Encrypt(userRequestDto.Password, signedUpUser.Salt)
            .Returns(signedUpUser.EncryptedPassword);
        
        jwtGenerator.Generate(userRequestDto.UserName).Returns(expected);

        // Act
        var actionResult = await sut.CreateSession(userRequestDto, token);

        // Assert
        Assert.IsType<JwtTokenDto>(actionResult.Value);
        Assert.Equal(expected, actionResult.Value.Token);
    }
}
