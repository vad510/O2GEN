using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2GEN.Authorization;
using O2GEN.Helpers;
using O2GEN.Models;
using O2GEN.Models.EmployeeModels;
using O2GEN.Models.HomeModels;
using System.Collections.Generic;
using System.Linq;

namespace O2GEN.Controllers
{
    [Authorize]
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


        [Route("Employee/Engineers")]
        [HttpGet]
        public IActionResult Engineers()
        {
            string DepartmentIdData = Request.Cookies["engdid"];
            int Dept = 0;
            if (!int.TryParse(DepartmentIdData, out Dept)) Dept = (int)((Credentials)HttpContext.Items["User"]).DeptId;
            if (Dept == 0)
            {
                var d = Helpers.DBHelper.GetChildDepartments();
                if (d.Count > 0) Dept = d[0].Id;
            }
            ViewBag.Engineers = Helpers.DBHelper.GetEngineers(DeptId: Dept, logger: _logger, UserDept: ((Credentials)HttpContext.Items["User"]).DeptId);
            Response.Cookies.Append("engdid", Dept.ToString());
            AlertHelper.DisplayMessage(HttpContext.Session, ViewBag);
            return View(new Filter() { DepartmentId = Dept });
        }
        [Route("Employee/Engineers")]
        [HttpPost]
        public IActionResult Engineers(Filter Model)
        {
            ViewBag.Engineers = Helpers.DBHelper.GetEngineers(DeptId: Model.DepartmentId, logger: _logger, UserDept: ((Credentials)HttpContext.Items["User"]).DeptId);
            Response.Cookies.Append("engdid", (Model.DepartmentId == null ? "" : Model.DepartmentId.ToString()));
            return View(Model);
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
            AlertModel resp = new AlertModel();
            if (!ModelState.IsValid)
            {
                return PartialView("EngineerEdit", Model);
            }
            if (Model.Id == -1)
            {
                Helpers.DBHelper.CreateEngineer(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Пользователь {Model.Surname} {Model.GivenName} добавлен.");
            }
            else
            {
                Helpers.DBHelper.UpdateEngineer(Model, ((Credentials)HttpContext.Items["User"]).Id, ((Credentials)HttpContext.Items["User"]).UserName, ((Credentials)HttpContext.Items["User"]).RoleCode, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Пользователь {Model.Surname} {Model.GivenName} обновлен.");
            }
            return new JsonResult(0); // RedirectToAction("Engineers");
        }
        [HttpGet]
        public IActionResult EngineerDelete(int Id)
        {
            AlertModel resp = new AlertModel();
            Helpers.DBHelper.DeleteEngineer(Id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, "Пользователь удален.");
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
        [Route("Employee/Resources")]
        [HttpGet]
        public IActionResult Resources()
        {
            string DepartmentIdData = Request.Cookies["resdid"];
            int Dept = 0;
            if (!int.TryParse(DepartmentIdData, out Dept)) Dept = (int)((Credentials)HttpContext.Items["User"]).DeptId;
            if (Dept == 0)
            {
                var d = Helpers.DBHelper.GetChildDepartments();
                if (d.Count > 0) Dept = d[0].Id;
            }
            ViewBag.Resources = Helpers.DBHelper.GetResources(DeptId: Dept, logger: _logger, UserDept: ((Credentials)HttpContext.Items["User"]).DeptId);
            Response.Cookies.Append("resdid", Dept.ToString());
            AlertHelper.DisplayMessage(HttpContext.Session, ViewBag);
            return View(new Filter() { DepartmentId = Dept });
        }
        [Route("Employee/Resources")]
        [HttpPost]
        public IActionResult Resources(Filter Model)
        {
            ViewBag.Resources = Helpers.DBHelper.GetResources(DeptId: Model.DepartmentId, logger: _logger, UserDept: ((Credentials)HttpContext.Items["User"]).DeptId);
            Response.Cookies.Append("resdid", (Model.DepartmentId == null ? "" : Model.DepartmentId.ToString()));
            return View(Model);
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
            res.Engineers = Helpers.DBHelper.GetResourceEngineers((long)res.DepartmentId, res.Id, _logger);
            if (res != null) return PartialView("ResourceEdit", res);
            return View();
        }
        [HttpGet]
        public IActionResult ResourceEditTable(long Id, long? DepartmentId)
        {
            Resource outpt = new Resource();
            if(DepartmentId is not null)
            {
                outpt.Engineers = Helpers.DBHelper.GetResourceEngineers((long)DepartmentId, Id, _logger);
            }
            return PartialView("ResourceEditTable", outpt);
        }


        [HttpPost]
        public IActionResult ResourceUpdate(Resource Model)
        {
            if (Model.Id == -1)
            { 
                Helpers.DBHelper.CreateResource(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Должность {Model.DisplayName} создана.");
            }
            else
            { 
                Helpers.DBHelper.UpdateResource(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Должность {Model.DisplayName} обновлена.");
            }
            return new JsonResult(0);  //return RedirectToAction("Resources");
        }
        [HttpGet]
        public IActionResult ResourceDelete(int id)
        {
            Helpers.DBHelper.DeleteResource(id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Должность удалена.");
            return RedirectToAction("Resources");
        }

        #region AJAX!

        [HttpGet]
        public JsonResult GetEngineersByDept(string DeptId)
        {
            int id = 0;
            if (int.TryParse(DeptId, out id))
                return new JsonResult(Helpers.DBHelper.GetEngineersList(id, _logger));
            else
                return new JsonResult(new List<Engineer>());
        }
        #endregion
    }
}
