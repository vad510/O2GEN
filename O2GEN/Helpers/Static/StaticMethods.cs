using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace O2GEN.Helpers.Static
{
    public static class StaticMethods
    {
        public static IHtmlContent DynamicNavigationLink(this IHtmlHelper html,
            string linkText)
        {
            string action = (string)html.ViewContext.RouteData.Values["action"];
            string controller = (string)html.ViewContext.RouteData.Values["controller"];

            if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(controller))
                return null;

            if (controller.Equals("Home") && action.Equals("Index"))
                    return null;

            var title = html.ViewData["Title"];

            if (title != null)
                return html.ActionLink(title.ToString(), action, controller, routeValues: null, htmlAttributes: new { @class = "nav-link-with-arrow" });

            return html.ActionLink("link", action, controller, routeValues: null, htmlAttributes: new { @class = "nav-link-with-arrow" });
        }
    }
}
