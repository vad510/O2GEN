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
                data.DisplayName = $"{reslt.Surname} {(!string.IsNullOrEmpty(reslt.GivenName) ? $"{reslt.GivenName.Substring(0, 1)}." : "")}{(!string.IsNullOrEmpty(reslt.MiddleName) ? $"{reslt.MiddleName.Substring(0, 1)}." : "")}";
                data.DeptId = reslt.DeptId;
                data.Id = reslt.Id;
                data.RoleCode = reslt.RoleCode;

                reslt.JWToken = JwtTokenExtension.GenerateJwtToken(data);
                CookieHelper.ClearAllCookies(Request.Cookies.Keys, Response);
                HttpContext.Session.SetString("token", reslt.JWToken);
                DBHelper.SignInLog(data, _logger);
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

        [Authorize]
        [Route("Account/Settings")]
        [HttpGet]
        public IActionResult Settings()
        {
            Engineer Data = DBHelper.GetEngineer((int)((Credentials)HttpContext.Items["User"]).Id, _logger);
            return View(Data);
        }

        [Authorize]
        [Route("Account/Settings")]
        [HttpPost]
        public IActionResult Settings(Engineer data)
        {
            if (ModelState.IsValid)
            {
                DBHelper.UpdateEngineer(data, ((Credentials)HttpContext.Items["User"]).Id, ((Credentials)HttpContext.Items["User"]).UserName, ((Credentials)HttpContext.Items["User"]).RoleCode, _logger);
                Credentials usData = (Credentials)HttpContext.Items["User"];
                usData.DeptId = (long)data.DepartmentId;
                usData.DisplayName = $"{data.Surname} {(!string.IsNullOrEmpty(data.GivenName) ? $"{data.GivenName.Substring(0, 1)}." : "")}{(!string.IsNullOrEmpty(data.MiddleName) ? $"{data.MiddleName.Substring(0, 1)}." : "")}";
                HttpContext.Session.SetString("token", JwtTokenExtension.GenerateJwtToken(usData));
                AlertHelper.DisplayMessage(ViewBag,AlertType.Success,"Данные сохранены.");
            }
            else
            {
                AlertHelper.DisplayMessage(ViewBag, AlertType.Warning, "Ошибка на сохранении.");
            }
            return View(data);
        }
    }
}
