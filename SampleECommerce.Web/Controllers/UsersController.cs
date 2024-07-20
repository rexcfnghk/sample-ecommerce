using Microsoft.AspNetCore.Mvc;

namespace SampleECommerce.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController
{
    [HttpGet("[action]")]
    public ActionResult<string> Test()
    {
        return "hi";
    }
}