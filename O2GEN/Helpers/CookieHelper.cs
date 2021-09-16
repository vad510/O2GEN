using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2GEN.Helpers
{
    public static class CookieHelper
    {
        /// <summary>
        /// Удаляем все не стандартные куки.
        /// </summary>
        /// <param name="Cookies"></param>
        /// <param name="Response"></param>
        public static void ClearAllCookies(ICollection<string> Cookies, HttpResponse Response)
        {
            foreach (string cookie in Cookies)
            {
                if (cookie == ".AspNetCore.Session") continue;
                Response.Cookies.Delete(cookie);
            }
        }
    }
}
