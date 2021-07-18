using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2GEN.Models;
using O2GEN.Models.EmployeeModels;
using O2GEN.Models.HomeModels;
using System.Linq;

namespace O2GEN.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;


        private EmployeeListModel _employeeListModels;
        private DepartmentListModel dps = new();

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
        

            _employeeListModels = new EmployeeListModel();

            _employeeListModels.EmployeeModels.Add(new EmployeeModel
            {
                VisibleName = "1",
                Department = "1",
                UserType = "1",
                Edit = "",
            });

            for (var j = 0; j < 3; j++)
            {
                DepartmentHeadModel departmentHeadModel = new()
                {
                    Name = "lukoil",
                };

                for (var i = 0; i < 3; i++)
                {
                    departmentHeadModel.DepartmentModels.Add(new DepartmentModel
                    {
                        DepartmentName = "name",
                    });
                }

                dps.DepartmentHeadModels.Add(departmentHeadModel);
            }
        }

        public IActionResult Engineers()
        {
            ViewBag.Engineers = Helpers.DBHelper.GetEngineers(_logger);
            return View(_employeeListModels);
        }
        [HttpGet]
        public IActionResult EngineerCreate()
        {
            O2GEN.Models.Engineer model = new();
            return PartialView("EngineerEdit", model);
        }
        [HttpGet]
        public IActionResult EngineerEdit(int id)
        {
            var res = Helpers.DBHelper.GetEngineer(id, _logger);
            if (res != null) return PartialView("EngineerEdit", res);
            return View();
        }

        [HttpPost]
        public IActionResult EngineerUpdate(Engineer Model)
        {
            if (Model.Id == -1)
                Helpers.DBHelper.CreateEngineer(Model, User.Identity.Name, _logger);
            else
                Helpers.DBHelper.UpdateEngineer(Model, User.Identity.Name, _logger);
            return RedirectToAction("Engineers");
        }
        [HttpGet]
        public IActionResult EngineerDelete(int Id)
        {
            Helpers.DBHelper.DeleteEngineer(Id, User.Identity.Name, _logger);
            return RedirectToAction("Engineers");
        }

        public IActionResult Roles()
        {
            ViewBag.Roles = Helpers.DBHelper.GetPPRoles(_logger);
            return View();
        }
        public IActionResult Calendar()
        {
            return View();
        }

        public IActionResult Resources()
        {
            ViewBag.Resources = Helpers.DBHelper.GetResources(_logger);
            return View();
        }

        [HttpGet]
        public IActionResult ResourceCreate()
        {
            Resource model = new();
            return PartialView("ResourceEdit", model);
        }
        [HttpGet]
        public IActionResult ResourceEdit(int id)
        {
            var res = Helpers.DBHelper.GetResource(id, _logger);
            if (res != null) return PartialView("ResourceEdit", res);
            return View();
        }

        [HttpPost]
        public IActionResult ResourceUpdate(Resource Model)
        {
            if (Model.Id == -1)
                Helpers.DBHelper.CreateResource(Model, User.Identity.Name, _logger);
            else
                Helpers.DBHelper.UpdateResource(Model, User.Identity.Name, _logger);
            return RedirectToAction("Resources");
        }
        [HttpGet]
        public IActionResult ResourceDelete(int id)
        {
            Helpers.DBHelper.DeleteResource(id, User.Identity.Name, _logger);
            return RedirectToAction("Resources");
        }
    }
}
