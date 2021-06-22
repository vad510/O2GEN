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
        public IActionResult ObjectCreate()
        {
            O2GEN.Models.Asset model = new();
            return PartialView("ObjectEdit", model);
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
        public IActionResult EmployeeCategoryCreate()
        {
            O2GEN.Models.PersonCategory model = new();
            return PartialView("EmployeeCategoryEdit", model);
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
        public IActionResult mployeePositionCreate()
        {
            O2GEN.Models.PersonPosition model = new();
            return PartialView("EmployeePositionEdit", model);
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
        public IActionResult ObjectClassCreate()
        {
            O2GEN.Models.AssetClass model = new();
            return PartialView("ObjectClassEdit", model);
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
        public IActionResult RouteCreate()
        {
            O2GEN.Models.AssetParameterSet model = new();
            return PartialView("RouteEdit", model);
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
        public IActionResult ControlsCreate()
        {
            O2GEN.Models.Control model = new();
            return PartialView("ControlEdit", model);
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
        public IActionResult TOTypeCreate()
        {
            O2GEN.Models.TOType model = new();
            return PartialView("TOTypeEdit", model);
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
        public IActionResult DepartmentsCreate()
        {
            O2GEN.Models.Department model = new();
            return PartialView("DepartmentEdit", model);
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
