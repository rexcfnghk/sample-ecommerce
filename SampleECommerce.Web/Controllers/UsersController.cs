using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.ModelBinders;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;

namespace SampleECommerce.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserSignupService userSignupService, IOrderService orderService, IProductService productService)
    : ControllerBase
{
    private readonly IUserSignupService _userSignupService = userSignupService;
    private readonly IOrderService _orderService = orderService;
    private readonly IProductService _productService = productService;

    [HttpPost]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Signup(
        UserRequestDto dto,
        CancellationToken token = default)
    {
        var request = new UserSignupRequest(dto.UserName, dto.Password);

        await _userSignupService.SignupAsync(request, token);

        return NoContent();
    }

    [HttpGet("Orders")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IDictionary<Guid, OrderDto>>> ListOrders(
        UserIdDto userId,
        CancellationToken token = default)
    {
        var orders = await _orderService.ListOrdersAsync(userId.UserId, token);

        var output = orders.ToDictionary(
            dto => dto.Id,
            dto => new OrderDto
            {
                OrderTime = dto.OrderTime,
                OrderItems = dto.OrderItems.Select(
                        oi => new OrderItemDto
                        {
                            ProductName = oi.Product.Name,
                            Quantity = oi.Quantity,
                            ProductCategory = oi.Product.Category,
                            ProductPrice = oi.Product.Price
                        })
                    .ToList()
            });

        return output;
    }

    [HttpPost("Orders")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderSuccessfulDto>> PostOrder(
        UserIdDto userId,
        PostOrderDto postOrderDto,
        CancellationToken token = default)
    {
        // Retrieve product from productId?
        // Then map to order model
        // Then try save to db
        var productIds = new HashSet<string>(
            postOrderDto.OrderItems.Select(oi => oi.ProductId!));

        var products =
            await _productService.GetProductsAsync(productIds, token);

        var orderItems =
            from orderItem in postOrderDto.OrderItems
            from product in products
            where orderItem.ProductId == product.Id
            select new OrderItem(product, orderItem.Quantity.Value);

        var orderItemList = orderItems.ToList();

        if (orderItemList.Count == 0)
        {
            return BadRequest("No order items submitted");
        }

        var orderId = Guid.NewGuid();
        var orderTime = DateTimeOffset.Now;

        var order = new Order(
            orderId,
            orderTime,
            orderItemList);

        await _orderService.PostOrderAsync(userId.UserId, order, token);

        return new OrderSuccessfulDto
            { OrderId = orderId, OrderTime = orderTime };
    }
}
