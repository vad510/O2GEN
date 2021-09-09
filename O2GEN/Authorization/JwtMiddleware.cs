using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace O2GEN.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string token = context.Session?.GetString("token");// context.Request.Headers["Authorization"].ToString();
            var user = JwtTokenExtension.ValidateJwtToken(token);
            if (user != null)
            {
                context.Items["User"] = user;
            }

            await _next(context);
        }
    }
}
