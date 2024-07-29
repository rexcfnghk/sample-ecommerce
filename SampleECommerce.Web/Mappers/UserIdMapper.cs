using System.Security.Claims;
using SampleECommerce.Web.Constants;
using SampleECommerce.Web.Models;

namespace SampleECommerce.Web.Mappers;

public class UserIdMapper : IMapper<ClaimsPrincipal, UserId>
{
    public UserId Map(ClaimsPrincipal source)
    {
        var claim = source.Claims.Single(c => c.Type == ClaimNames.UserId);
        var userId = int.Parse(claim.Value);
        return (UserId)userId;
    }
}
