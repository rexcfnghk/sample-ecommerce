using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.Controllers;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Jwt;

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
        [Frozen] IJwtGenerator jwtGenerator,
        SessionsController sut)
    {
        jwtGenerator.Generate(userRequestDto.UserName).Returns(expected);

        var actionResult = await sut.CreateSession(userRequestDto);

        Assert.IsType<JwtTokenDto>(actionResult.Value);
        Assert.Equal(expected, actionResult.Value.Token);
    }
}
