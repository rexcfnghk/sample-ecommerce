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
        IReadOnlyList<Order> orders,
        UserIdDto dto,
        UsersController sut,
        CancellationToken token)
    {
        mockOrderService.ListOrdersAsync(dto.UserId, token).Returns(Task.FromResult(orders));
        var expected = orders.ToDictionary(
            o => o.Id,
            o => new OrderDto
            {
                OrderTime = o.OrderTime,
                OrderItems = o.OrderItems.Select(
                        oi => new OrderItemDto
                        {
                            ProductName = oi.Product.Name,
                            Quantity = oi.Quantity,
                            ProductCategory = oi.Product.Category,
                            ProductPrice = oi.Product.Price
                        })
                    .ToList()
            });

        var result = await sut.ListOrders(dto, token);

        Assert.Equivalent(expected, result.Value);
    }
}
