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
    public async Task Signup_ReturnsExpectedJwtToken(
        JwtToken expected,
        [Frozen] IUserSignupService mockUserSignupService,
        UserRequestDto dto,
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

        Assert.IsType<NoContentResult>(result);
    }

    [Theory, AutoNSubstituteData]
    public async Task ListOrders_ReturnsResultFromIOrderService(
        [Frozen] IOrderService mockOrderService,
        IEnumerable<OrderItem> orders,
        IReadOnlyList<IGrouping<Guid, OrderDto>> expected,
        UserIdDto dto,
        UsersController sut,
        CancellationToken token)
    {
        var groupedOrders = from order in orders group order by order.Id;
        mockOrderService.ListOrdersAsync(dto.UserId, token).Returns(Task.FromResult(groupedOrders));

        var result = await sut.ListOrders(dto, token);

        Assert.Equivalent(expected, result);
    }
}
