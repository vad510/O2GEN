using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using O2GEN.Models;
using System;
using System.Linq;

namespace O2GEN.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            var user = (Credentials)context.HttpContext.Items["User"];
            switch (user.TokenException)
            {
                case TokenExceprion.Ok:
                    context.HttpContext.Response.Headers.Add("Authorization", user.Token);
                    break;
                case TokenExceprion.Expired:
                    //context.Result = new JsonResult(user) { StatusCode = StatusCodes.Status401Unauthorized };
                    context.HttpContext.Response.Headers.Add("Authorization", user.Token);
                    break;
                case TokenExceprion.WrongToken:
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                    //context.Result = new JsonResult(user) { StatusCode = StatusCodes.Status401Unauthorized };
                    break;
                default:
                    break;
            }
        }
    }
}
