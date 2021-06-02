using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2GEN.Models;
using System.Diagnostics;

namespace O2GEN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private List<TestClass> _test;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
<<<<<<< HEAD
            return View();
=======

            _test = new List<TestClass>();

            _test.Add(new TestClass
            {
                EquipmentType = "1",
                Status = "2",
                Type = "3",
                Bypass = "4",
                Bypasser = "5",
                Start = DateTime.Now,
                End = DateTime.Now
            });
>>>>>>> 5e9eb3f23678bd0cf1a7caa4bfc01ff5b6d34e74
            if (User.Identity.IsAuthenticated)
            {
                return View(_test);
            }
            else
            {
                return Redirect("/Account/login");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
