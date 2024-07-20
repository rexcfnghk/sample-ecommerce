using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.Controllers;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Tests.Controllers;

public class UsersControllerTests
{
    [Theory, AutoNSubstituteData]
    public void Test_ReturnsExpectedString(UsersController sut)
    {
        var result = sut.Test();

        Assert.Equal("hi", result.Value);
    }

    [Theory, AutoNSubstituteData]
    public async Task Signup_ReturnsExpectedJwtToken(
        JwtToken expected,
        [Frozen] IUserSignupService mockUserSignupService,
        UserSignupRequestDto dto,
        UsersController sut,
        CancellationToken token)
    {
        mockUserSignupService.SignupAsync(
                Arg.Is<UserSignupRequest>(
                    r => r.Username == dto.UserName &&
                         r.Password == dto.Password),
                token)
            .Returns(Task.FromResult(expected));
        
        var result = await sut.Signup(dto, token);

        Assert.IsType<OkObjectResult>(result.Result);
        var okObjectResult = (OkObjectResult)result.Result;
        Assert.Equal(expected, okObjectResult.Value);
    }
}