using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2GEN.Models;
using O2GEN.Models.DeskModels;
using System.Diagnostics;

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

        [HttpPost]
        public IActionResult ObjectUpdate(Asset Model)
        {
            if (Model.Id == -1)
                Helpers.DBHelper.CreateAsset(Model, User.Identity.Name, _logger);
            else
                Helpers.DBHelper.UpdateAsset(Model, User.Identity.Name, _logger);
            return RedirectToAction("Objects");
        }
        [HttpGet]
        public IActionResult ObjectDelete(int Id)
        {
            Helpers.DBHelper.DeleteAsset(Id, User.Identity.Name, _logger);
            return RedirectToAction("Objects");
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

        [HttpPost]
        public IActionResult EmployeeCategoryUpdate(PersonCategory Model)
        {
            if (Model.Id == -1)
                Helpers.DBHelper.CreatePersonCategory(Model, User.Identity.Name, _logger);
            else
                Helpers.DBHelper.UpdatePersonCategory(Model, User.Identity.Name, _logger);
            return RedirectToAction("EmployeeCategory");
        }
        [HttpGet]
        public IActionResult EmployeeCategoryDelete(int Id)
        {
            Helpers.DBHelper.DeletePersonCategory(Id, User.Identity.Name, _logger);
            return RedirectToAction("EmployeeCategory");
        }

        public IActionResult EmployeePosition()
        {
            ViewBag.EmployeePositions = Helpers.DBHelper.GetPersonPositions(_logger);
            return View();
        }
        [HttpGet]
        public IActionResult EmployeePositionCreate()
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
        [HttpPost]
        public IActionResult EmployeePositionUpdate(PersonPosition Model)
        {
            if (Model.Id == -1)
            {
                Helpers.DBHelper.CreatePersonPosition(Model, User.Identity.Name, _logger);
            }
            else
            {
                Helpers.DBHelper.UpdatePersonPosition(Model, User.Identity.Name, _logger);
            }
            return RedirectToAction("EmployeePosition");
        }
        [HttpGet]
        public IActionResult EmployeePositionDelete(int Id)
        {
            Helpers.DBHelper.DeletePersonPosition(Id, User.Identity.Name, _logger);
            return RedirectToAction("EmployeePosition");
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

        [HttpPost]
        public IActionResult ObjectClassUpdate(AssetClass Model)
        {
            if (Model.Id == -1)
                Helpers.DBHelper.CreateAssetClass(Model, User.Identity.Name, _logger);
            else
                Helpers.DBHelper.UpdateAssetClass(Model, User.Identity.Name, _logger);
            return RedirectToAction("ObjectClass");
        }

        [HttpGet]
        public IActionResult ObjectClassDelete(int Id)
        {
            Helpers.DBHelper.DeleteAssetClass(Id, User.Identity.Name, _logger);
            return RedirectToAction("ObjectClass");
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
        public IActionResult ControlCreate()
        {
            Control model = new();
            return PartialView("ControlEdit", model);
        }

        [HttpGet]
        public IActionResult ControlEdit(int id)
        {
            var res = Helpers.DBHelper.GetControl(id, _logger);
            if (res != null) return PartialView("ControlEdit", res);
            return View();
        }

        [HttpPost]
        public IActionResult ControlUpdate(Control Model)
        {
            if (!ModelState.IsValid)
            {
                Debug.Write("Model state not valid");
                //
                return PartialView("ControlEdit", Model);
            }
            Debug.Write("Model state is valid");
            //return RedirectToAction("Controls", "Desk");

            if (Model.Id == -1)
            {
                Helpers.DBHelper.CreateControl(Model, User.Identity.Name, _logger);
            }
            else
            {
                Helpers.DBHelper.UpdateControl(Model, User.Identity.Name, _logger);
            }
            return RedirectToAction("Controls");
        }

        [HttpGet]
        public IActionResult ControlDelete(int Id)
        {
            Helpers.DBHelper.DeleteControl(Id, User.Identity.Name, _logger);
            return RedirectToAction("Controls");
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
            ViewBag.Departments = Helpers.DBHelper.GetDepartments(logger: _logger);

            ControlListModel model = new ControlListModel();
            model.ControlModels = new System.Collections.Generic.List<ControlModel>();
            for (var i = 0; i < 10; i++)
            {
                var ms = new ControlModel()
                {
                    Name = "Name"
                };

                ms.Childs = new System.Collections.Generic.List<ControlModel>();

                model.ControlModels.Add(ms);

                for (var j = 0; j < 5; j++)
                {
                    ms.Childs.Add(new ControlModel()
                    {
                        Name = "NNN"
                    });
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DepartmentCreate()
        {
            Department model = new();
            return PartialView("DepartmentEdit", model);
        }

        [HttpGet]
        public IActionResult DepartmentEdit(int id)
        {
            var res = Helpers.DBHelper.GetDepartment(id, _logger);

            if (res != null)
                return PartialView("DepartmentEdit", res);
            return View();
        }

        [HttpPost]
        public IActionResult DepartmentUpdate(Department Model)
        {
            if (Model.Id == -1)
                Helpers.DBHelper.CreateDepartment(Model, User.Identity.Name, _logger);
            else
                Helpers.DBHelper.UpdateDepartment(Model, User.Identity.Name, _logger);
            return RedirectToAction("Departments");
        }

        [HttpGet]
        public IActionResult DepartmentDelete(int Id)
        {
            Helpers.DBHelper.DeleteDepartment(Id, User.Identity.Name, _logger);
            return RedirectToAction("Departments");
        }
    }
}
