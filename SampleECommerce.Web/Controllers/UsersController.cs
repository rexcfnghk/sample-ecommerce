using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SampleECommerce.Web.Constants;
using SampleECommerce.Web.Dtos;
using SampleECommerce.Web.ModelBinders;
using SampleECommerce.Web.Models;
using SampleECommerce.Web.Services;
using SampleECommerce.Web.Swashbuckle;

namespace SampleECommerce.Web.Controllers;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class UsersController(IUserSignupService userSignupService, IOrderService orderService, IProductService productService)
    : ControllerBase
{
    private readonly IUserSignupService _userSignupService = userSignupService;
    private readonly IOrderService _orderService = orderService;
    private readonly IProductService _productService = productService;

    /// <summary>
    /// Creates a user
    /// </summary>
    /// <param name="dto">The username and password combination for the user to be created</param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <response code="204">A user is successfully created</response>
    /// <response code="400">When there are missing field(s), or the user is already in the system</response>
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

    /// <summary>
    /// Lists the order for a user
    /// </summary>
    /// <param name="userId">The userId of the user, passed in the payload of the JWT</param>
    /// <param name="token"></param>
    /// <returns>A list of orders made by the user</returns>
    /// <response code="200">The orders that the user made</response>
    /// <response code="400">When there are missing field(s), or the request is malformed</response>
    /// <response code="403">The system cannot authenticate/authorize the user</response>
    [HttpGet("Orders")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Bearer )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IDictionary<Guid, OrderDto>>> ListOrders(
        [OpenApiParameterIgnore] UserIdDto userId,
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

    /// <summary>
    /// Place an order for a user
    /// </summary>
    /// <param name="userId">The userId of the user, passed in the payload of the JWT</param>
    /// <param name="postOrderDto">The payload for the order</param>
    /// <param name="token"></param>
    /// <response code="200">The order that the user made</response>
    /// <response code="400">When there are missing field(s), or the request is malformed</response>
    /// <response code="401">The system cannot authenticate the user</response>
    /// <returns>The order that the user made</returns>
    [HttpPost("Orders")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Bearer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderSuccessfulDto>> PostOrder(
        [OpenApiParameterIgnore] UserIdDto userId,
        PostOrderDto postOrderDto,
        CancellationToken token = default)
    {
        var productIds = new HashSet<string>(
            postOrderDto.OrderItems.Select(oi => oi.ProductId!));

        var products =
            await _productService.GetProductsAsync(productIds, token);

        if (products.Count == 0)
        {
            var ids = string.Join(',', productIds.Except(products.Select(p => p.Id)));
            return BadRequest($"Could not find products with Ids: {ids}");
        }

        var orderItems =
            from orderItem in postOrderDto.OrderItems
            from product in products
            where orderItem.ProductId == product.Id && orderItem.Quantity > 0
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
