using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using SampleECommerce.Tests.Attributes;
using SampleECommerce.Web.Controllers;

namespace SampleECommerce.Tests;

public class UsersControllerTests
{
    [Theory, AutoNSubstituteData]
    public void Test_ReturnsExpectedString(UsersController sut)
    {
        var result = sut.Test();

        Assert.Equal("hi", result.Value);
    }
}