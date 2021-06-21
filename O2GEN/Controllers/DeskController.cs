using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2GEN.Models.DeskModels;

namespace O2GEN.Controllers
{
    public class DeskController : Controller
    {
        private readonly ILogger<DeskController> _logger;
        ObjectListModel obj = new();
        public DeskController(ILogger<DeskController> logger)
        {
            _logger = logger;
        }

        public IActionResult Objects()
        {
            ViewBag.Objects = Helpers.DBHelper.GetAssets(_logger);
            return View(obj);
        }

        [HttpGet]
        public IActionResult CreateObject()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ObjectEdit(int id)
        {
            var res = Helpers.DBHelper.GetAssets(id, _logger);
            if (res != null) return PartialView("ObjectEdit", res);
            return View();
        }


        public IActionResult EmployeeCategory()
        {
            ViewBag.EmployeeCategories = Helpers.DBHelper.GetPersonCategories(_logger);
            return View();
        }
        [HttpGet]
        public IActionResult EmployeeCategoryEdit(int id)
        {
            var res = Helpers.DBHelper.GetPersonCategory(id, _logger);
            if (res != null) return PartialView("EmployeeCategoryEdit", res);
            return View();
        }

        public IActionResult EmployeePosition()
        {
            ViewBag.EmployeePositions = Helpers.DBHelper.GetPersonPositions(_logger);
            return View();
        }
        [HttpGet]
        public IActionResult EmployeePositionEdit(int id)
        {
            var res = Helpers.DBHelper.GetPersonPosition(id, _logger);
            if (res != null) return PartialView("EmployeePositionEdit", res);
            return View();
        }

        public IActionResult ObjectClass()
        {
            ViewBag.ObjectClasses = Helpers.DBHelper.GetAssetClasses(_logger);
            return View();
        }
        [HttpGet]
        public IActionResult ObjectClassEdit(int id)
        {
            var res = Helpers.DBHelper.GetAssetClass(id, _logger);
            if (res != null) return PartialView("ObjectClassEdit", res);
            return View();
        }

        public IActionResult Route()
        {
            ViewBag.Routes = Helpers.DBHelper.GetAssetParameterSets(_logger);
            return View();
        }
        [HttpGet]
        public IActionResult RouteEdit(int id)
        {
            var res = Helpers.DBHelper.GetAssetParameterSet(id, _logger);
            if (res != null) return PartialView("RouteEdit", res);
            return View();
        }

        public IActionResult Controls()
        {
            ViewBag.Controls = Helpers.DBHelper.GetControls(_logger);
            return View();
        }
        [HttpGet]
        public IActionResult ControlEdit(int id)
        {
            var res = Helpers.DBHelper.GetControl(id, _logger);
            if (res != null) return PartialView("ControlEdit", res);
            return View();
        }

        public IActionResult TOTypes()
        {
            ViewBag.TOTypes = Helpers.DBHelper.GetTOTypes(_logger);
            return View();
        }
        [HttpGet]
        public IActionResult TOTypeEdit(int id)
        {
            var res = Helpers.DBHelper.GetTOType(id, _logger);

            if (res != null)
                return PartialView("TOTypeEdit", res);
            return View();
        }

        public IActionResult Departments()
        {
            ViewBag.Departments = Helpers.DBHelper.GetDepartments(_logger);
            return View();
        }

        [HttpGet]
        public IActionResult DepartmentsEdit(int id)
        {
            var res = Helpers.DBHelper.GetDepartment(id, _logger);

            if (res != null)
                return PartialView("DepartmentEdit", res);
            return View();
        }
    }
}
