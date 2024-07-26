using System.Collections.ObjectModel;
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
    public async Task Signup_CallsUserSignUpService(
        [Frozen] IUserSignupService mockUserSignupService,
        UserRequestDto dto,
        UsersController sut,
        CancellationToken token)
    {
        // Arrange
        mockUserSignupService.SignupAsync(
                Arg.Is<UserSignupRequest>(
                    r => r.Username == dto.UserName &&
                         r.Password == dto.Password),
                token)
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await sut.Signup(dto, token);

        // Assert
        mockUserSignupService.Received(1)
            .SignupAsync(
                Arg.Is<UserSignupRequest>(
                    r => r.Username == dto.UserName &&
                         r.Password == dto.Password),
                token);
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

    [Theory, AutoNSubstituteData]
    public async Task
        PostOrder_ReturnsBadRequest_WhenProductIdsCannotBeResolvedIntoProducts(
            UserIdDto userId,
            PostOrderDto postOrderDto,
            [Frozen] IProductService mockProductService,
            UsersController sut,
            CancellationToken cancellationToken)
    {
        mockProductService
            .GetProductsAsync(
                Arg.Any<ISet<string>>(),
                Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IReadOnlyList<Product>>(new List<Product>()));

        var result = await sut.PostOrder(
            userId,
            postOrderDto,
            cancellationToken);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Theory, AutoNSubstituteData]
    public async Task
        PostOrder_ReturnsBadRequest_WhenOneOrderItemHasQuantityEqualsToZero(
            UserIdDto userId,
            PostOrderDto postOrderDto,
            IReadOnlyList<Product> products,
            [Frozen] IProductService mockProductService,
            UsersController sut,
            CancellationToken cancellationToken)
    {
        postOrderDto.OrderItems[0].Quantity = 0;
        mockProductService
            .GetProductsAsync(
                Arg.Any<ISet<string>>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(products));

        var result = await sut.PostOrder(
            userId,
            postOrderDto,
            cancellationToken);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
    
    [Theory, AutoNSubstituteData]
    public async Task
        PostOrder_ReturnsExpectedDto_WhenOrderItemsCanBeProccessed(
            UserIdDto userId,
            IReadOnlyList<Product> products,
            Order order,
            [Frozen] IProductService mockProductService,
            [Frozen] IOrderService mockOrderService,
            UsersController sut,
            CancellationToken cancellationToken)
    {
        var postOrderDto = new PostOrderDto
        {
            OrderItems = products
                .Select(p => new PostOrderItemDto { ProductId = p.Id, Quantity = 10 })
                .ToList()
        };
        
        mockProductService
            .GetProductsAsync(
                Arg.Any<ISet<string>>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(products));
        mockOrderService.GenerateOrder(Arg.Any<IReadOnlyList<OrderItem>>())
            .Returns(order);

        var result = await sut.PostOrder(
            userId,
            postOrderDto,
            cancellationToken);

        Assert.NotNull(result.Value);
        Assert.Equal(order.Id, result.Value.OrderId);
        Assert.Equal(order.OrderTime, result.Value.OrderTime);
    }
}
