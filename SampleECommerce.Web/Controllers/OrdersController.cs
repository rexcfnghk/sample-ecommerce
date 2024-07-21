using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleECommerce.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public ActionResult<string> Test()
    {
        return string.Empty;
    }
}
