using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreatingCompetitionLists.Controllers
{
    [ApiController]
    [Route("user")]
    [AllowAnonymous]
    public class UserController : Controller
    {
        [Route("is-authenticated")]
        public string IsAuthenticated()
        {
            return User.Claims.FirstOrDefault(claim => claim.Value.Contains("@"))?.Value ?? "not authenticated";
        }
    }
}