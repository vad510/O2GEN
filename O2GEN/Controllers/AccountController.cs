using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2GEN.Models;
using O2GEN.Helpers;
using Microsoft.AspNetCore.Http;
using O2GEN.Authorization;
using Microsoft.AspNetCore.Routing;

namespace O2GEN.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [Route("Account/Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View(new Credentials());
        }

        [AllowAnonymous]
        [Route("Account/Login")]
        [HttpPost]
        public IActionResult Login(Credentials data)
        {
            if (ModelState.IsValid)
            {
                UserData reslt = DBHelper.GetUserData(data, _logger);
                if (reslt == null)
                {
                    ViewBag.PasswordException = "Логин или пароль не верны.";
                    data.Password = "";
                    return View(data);
                }

                data.DeptId = reslt.DeptId;
                data.Id = reslt.Id;

                reslt.JWToken = JwtTokenExtension.GenerateJwtToken(data);
                //HttpContext.Response.Headers.Add("Authorization", reslt.JWToken);
                //return View(data);
                //return Redirect("/Home/Index");
                CookieHelper.ClearAllCookies(Request.Cookies.Keys, Response);
                HttpContext.Session.SetString("token", reslt.JWToken);
                return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }
            return View(data);
        }

        [AllowAnonymous]
        [Route("Account/Logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("token");
            return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
        }
    }
}
