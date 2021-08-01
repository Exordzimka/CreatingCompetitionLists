using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreatingCompetitionLists.Controllers
{
    [ApiController]
    [Route("authentication")]
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");

            var properties = new AuthenticationProperties {RedirectUri = Url.Action("GoogleSignin")};
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        
        [Route("google-logout")]
        public IActionResult GoogleLogout()
        {
            return Redirect(User.Identity.IsAuthenticated == false ? "/" : "google-signout");
        }

        [Route("google-signin")]
        public async Task<IActionResult> GoogleSignin()
        {
            var context = HttpContext;
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
        
        [Route("google-signout")]
        public async Task<IActionResult> GoogleSignout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}