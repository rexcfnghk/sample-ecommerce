﻿using System.Globalization;
using System.Security.Claims;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.Constants;
using SampleECommerce.Web.Controllers;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.Jwt;
using SampleECommerce.Web.Mappers;
using SampleECommerce.Web.Models;

namespace SampleECommerce.Tests.Controllers;

public class SessionsControllerTests
{
    [Theory, AutoNSubstituteData]
    public void IsAssignableFromControllerBase(SessionsController sut)
    {
        Assert.IsAssignableFrom<ControllerBase>(sut);
    }

    [Theory, AutoNSubstituteData]
    public void CreateSession_GeneratesExpectedJwt(
        int userId,
        string expected,
        [Frozen] ClaimsPrincipal principal,
        [Frozen] HttpContext httpContext, 
        [Frozen] IMapper<ClaimsPrincipal, UserId> userIdMapper,
        [Frozen] IJwtGenerator jwtGenerator,
        SessionsController sut)
    {
        // Arrange
        principal.AddIdentity(
            new ClaimsIdentity(
                new List<Claim>
                {
                    new(
                        ClaimNames.UserId,
                        userId.ToString(CultureInfo.InvariantCulture))
                }));
        userIdMapper.Map(principal).Returns((UserId)userId);
        jwtGenerator.Generate(userId).Returns(expected);
        httpContext.User = principal;

        // Act
        var actionResult = sut.CreateSession();

        // Assert
        Assert.IsType<JwtTokenDto>(actionResult.Value);
        Assert.Equal(expected, actionResult.Value.Token);
    }
}
