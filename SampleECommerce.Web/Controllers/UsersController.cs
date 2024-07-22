﻿using System.Collections.ObjectModel;
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
public class UsersController(IUserSignupService userSignupService, IOrderService orderService)
    : ControllerBase
{
    private readonly IUserSignupService _userSignupService = userSignupService;
    private readonly IOrderService _orderService = orderService;

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
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> ListOrders(
        UserIdDto userId,
        CancellationToken token = default)
    {
        var orders = await _orderService.ListOrdersAsync(userId.UserId, token);
        var dtos =
            new ReadOnlyCollection<OrderDto>(
                orders.Select(o => new OrderDto()).ToList());
        return dtos;
    }
}
