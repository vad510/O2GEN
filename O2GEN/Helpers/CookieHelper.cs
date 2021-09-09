using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2GEN.Helpers
{
    /// <summary>
    /// Тут находится список всех куки используемых в фильтрах
    /// </summary>
    public static class CookieHelper
    {
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
