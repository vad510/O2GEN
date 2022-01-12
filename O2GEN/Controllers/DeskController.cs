using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2GEN.Authorization;
using O2GEN.Helpers;
using O2GEN.Models;
using System;
using System.Collections.Generic;

namespace O2GEN.Controllers
{
    [Authorize]
    public class DeskController : Controller
    {
        private readonly ILogger<DeskController> _logger;
        public DeskController(ILogger<DeskController> logger)
        {
            _logger = logger;
        }

        [Route("Desk/Objects")]
        [HttpGet]
        public IActionResult Objects()
        {
            string DepartmentIdData = Request.Cookies["odid"];
            string DisplayName = Request.Cookies["on"];
            int Dept = 0;
            if (!int.TryParse(DepartmentIdData, out Dept)) Dept = (int)((Credentials)HttpContext.Items["User"]).DeptId;
            if (Dept == 0)
            {
                var d = Helpers.DBHelper.GetChildDepartments();
                if (d.Count > 0) Dept = d[0].Id;
            }
            ViewBag.Objects = Helpers.DBHelper.GetAssets(logger: _logger, DeptID: Dept, DisplayName: DisplayName, UserDept: ((Credentials)HttpContext.Items["User"]).DeptId) ;
            Response.Cookies.Append("odid", Dept.ToString());
            Response.Cookies.Append("on", (string.IsNullOrEmpty(DisplayName) ? "" : DisplayName));
            AlertHelper.DisplayMessage(HttpContext.Session, ViewBag);
            return View(new Filter() { DepartmentId = Dept });
        }
        [Route("Desk/Objects")]
        [HttpPost]
        public IActionResult Objects(Filter Model)
        {
            ViewBag.Objects = Helpers.DBHelper.GetAssets(logger: _logger, DeptID: Model.DepartmentId, DisplayName: Model.DisplayName, UserDept: ((Credentials)HttpContext.Items["User"]).DeptId);
            Response.Cookies.Append("odid", (Model.DepartmentId == null ? "" : Model.DepartmentId.ToString()));
            Response.Cookies.Append("on", (string.IsNullOrEmpty(Model.DisplayName) ? "" : Model.DisplayName));
            return View(Model);
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


        [HttpGet]
        public IActionResult ObjectControlsEdit(long DeptId, long AssetId)
        {
            Asset outpt = new Asset();
            outpt.Parameters = Helpers.DBHelper.GetAssetParameterForAsset(DeptId, AssetId, _logger);
            return PartialView("ObjectControlsEdit", outpt);
        }

        [HttpPost]
        public IActionResult ObjectUpdate(Asset Model)
        {
            if (Model.Id == -1)
            { 
                Helpers.DBHelper.CreateAsset(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Объект {Model.DisplayName} добавлен.");
            }
            else
            {
                Helpers.DBHelper.UpdateAsset(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Объект {Model.DisplayName} обновлен.");
            }
            return new JsonResult(0);  //return RedirectToAction("Objects");
        }
        [HttpGet]
        public IActionResult ObjectDelete(int Id)
        {
            Helpers.DBHelper.DeleteAsset(Id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Объект удален.");
            return RedirectToAction("Objects");
        }


        [Obsolete]
        public IActionResult EmployeeCategory()
        {
            ViewBag.EmployeeCategories = Helpers.DBHelper.GetPersonCategories(_logger);
            AlertHelper.DisplayMessage(HttpContext.Session, ViewBag);
            return View();
        }
        [Obsolete]
        [HttpGet]
        public IActionResult EmployeeCategoryCreate()
        {
            O2GEN.Models.PersonCategory model = new();
            return PartialView("EmployeeCategoryEdit", model);
        }
        [Obsolete]
        [HttpGet]
        public IActionResult EmployeeCategoryEdit(int id)
        {
            var res = Helpers.DBHelper.GetPersonCategory(id, _logger);
            if (res != null) return PartialView("EmployeeCategoryEdit", res);
            return View();
        }

        [Obsolete]
        [HttpPost]
        public IActionResult EmployeeCategoryUpdate(PersonCategory Model)
        {
            if (Model.Id == -1)
                Helpers.DBHelper.CreatePersonCategory(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            else
                Helpers.DBHelper.UpdatePersonCategory(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            return RedirectToAction("EmployeeCategory");
        }
        [Obsolete]
        [HttpGet]
        public IActionResult EmployeeCategoryDelete(int Id)
        {
            Helpers.DBHelper.DeletePersonCategory(Id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            return RedirectToAction("EmployeeCategory");
        }

        [Obsolete]
        public IActionResult EmployeePosition()
        {
            ViewBag.EmployeePositions = Helpers.DBHelper.GetPersonPositions(_logger);
            AlertHelper.DisplayMessage(HttpContext.Session, ViewBag);
            return View();
        }
        [Obsolete]
        [HttpGet]
        public IActionResult EmployeePositionCreate()
        {
            O2GEN.Models.PersonPosition model = new();
            return PartialView("EmployeePositionEdit", model);
        }
        [Obsolete]
        [HttpGet]
        public IActionResult EmployeePositionEdit(int id)
        {
            var res = Helpers.DBHelper.GetPersonPosition(id, _logger);
            if (res != null) return PartialView("EmployeePositionEdit", res);
            return View();
        }
        [Obsolete]
        [HttpPost]
        public IActionResult EmployeePositionUpdate(PersonPosition Model)
        {
            if (Model.Id == -1)
            {
                Helpers.DBHelper.CreatePersonPosition(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            }
            else
            {
                Helpers.DBHelper.UpdatePersonPosition(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            }
            return RedirectToAction("EmployeePosition");
        }
        [Obsolete]
        [HttpGet]
        public IActionResult EmployeePositionDelete(int Id)
        {
            Helpers.DBHelper.DeletePersonPosition(Id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            return RedirectToAction("EmployeePosition");
        }



        [Obsolete]
        [Route("Desk/ObjectClass")]
        [HttpGet]
        public IActionResult ObjectClass()
        {
            string DisplayName = Request.Cookies["ocn"];
            ViewBag.Data = Helpers.DBHelper.GetAssetClasses(DisplayName: DisplayName, _logger);
            Response.Cookies.Append("ocn", (string.IsNullOrEmpty(DisplayName) ? "" : DisplayName));
            AlertHelper.DisplayMessage(HttpContext.Session, ViewBag);
            return View(new AssetClassFilter() { DisplayName = DisplayName });
        }
        [Obsolete]
        [Route("Desk/ObjectClass")]
        [HttpPost]
        public IActionResult ObjectClass(AssetClassFilter Model)
        {
            ViewBag.Data = Helpers.DBHelper.GetAssetClasses(DisplayName: Model.DisplayName, _logger);
            Response.Cookies.Append("ocn", (string.IsNullOrEmpty(Model.DisplayName) ? "" : Model.DisplayName));
            return View(Model);
        }

        [Obsolete]
        [HttpGet]
        public IActionResult ObjectClassCreate()
        {
            O2GEN.Models.AssetClass model = new();
            return PartialView("ObjectClassEdit", model);
        }
        [Obsolete]
        [HttpGet]
        public IActionResult ObjectClassEdit(int id)
        {
            var res = Helpers.DBHelper.GetAssetClass(id, _logger);
            if (res != null) return PartialView("ObjectClassEdit", res);
            return View();
        }

        [Obsolete]
        [HttpPost]
        public IActionResult ObjectClassUpdate(AssetClass Model)
        {
            if (Model.Id == -1)
                Helpers.DBHelper.CreateAssetClass(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            else
                Helpers.DBHelper.UpdateAssetClass(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            return RedirectToAction("ObjectClass");
        }

        [Obsolete]
        [HttpGet]
        public IActionResult ObjectClassDelete(int Id)
        {
            Helpers.DBHelper.DeleteAssetClass(Id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            return RedirectToAction("ObjectClass");
        }


        [Route("Desk/Route")]
        [HttpGet]
        public IActionResult Route()
        {
            string DepartmentIdData = Request.Cookies["rdid"];
            int Dept = 0;

            if (!int.TryParse(DepartmentIdData, out Dept)) Dept = (int)((Credentials)HttpContext.Items["User"]).DeptId;
            if (Dept == 0)
            {
                var d = Helpers.DBHelper.GetChildDepartments();
                if (d.Count > 0) Dept = d[0].Id;
            }
            ViewBag.Routes = Helpers.DBHelper.GetAssetParameterSets(UserDept: ((Credentials)HttpContext.Items["User"]).DeptId, logger:_logger, DeptID: Dept);
            Response.Cookies.Append("rdid", Dept.ToString());
            AlertHelper.DisplayMessage(HttpContext.Session, ViewBag);
            return View(new Filter() { DepartmentId = Dept });
        }
        [Route("Desk/Route")]
        [HttpPost]
        public IActionResult Route(Filter Model)
        {
            ViewBag.Routes = Helpers.DBHelper.GetAssetParameterSets(UserDept: ((Credentials)HttpContext.Items["User"]).DeptId, _logger, Model.DepartmentId);
            Response.Cookies.Append("rdid", (Model.DepartmentId == null ? "" : Model.DepartmentId.ToString()));
            return View(Model);
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

        [HttpPost]
        public IActionResult RouteUpdate(AssetParameterSet Model)
        {
            if (Model.Id == -1)
            { 
                Helpers.DBHelper.CreateAssetParameterSet(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Маршрут {Model.DisplayName} добавлен.");
            }
            else
            { 
                Helpers.DBHelper.UpdateAssetParameterSet(Model, ((Credentials) HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Маршрут {Model.DisplayName} обновлен.");
            }
            return new JsonResult(0);  //return RedirectToAction("Route");
        }

        [HttpGet]
        public IActionResult RouteDelete(int Id)
        {
            Helpers.DBHelper.DeleteAssetParameterSet(Id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Маршрут удален.");
            return RedirectToAction("Route");
        }

        [Route("Desk/Controls")]
        [HttpGet]
        public IActionResult Controls()
        {
            string SAssetParameterTypeId = Request.Cookies["aptid"];
            string DisplayName = Request.Cookies["apdn"];
            string DepartmentIdData = Request.Cookies["apdid"];
            long Type = 0;
            long.TryParse(SAssetParameterTypeId, out Type);
            long DepartmentId = 0;
            long.TryParse(DepartmentIdData, out DepartmentId);
            ViewBag.Data = Helpers.DBHelper.GetControls(AssetParameterTypeId: (Type == 0 ? null : Type), DisplayName: DisplayName, DeptId: (DepartmentId == 0 ? null : DepartmentId), logger: _logger);
            Response.Cookies.Append("aptid", Type == 0 ? "" : Type.ToString());
            Response.Cookies.Append("apdn", (string.IsNullOrEmpty(DisplayName) ? "" : DisplayName));
            Response.Cookies.Append("apdid", Type == 0 ? "" :DepartmentId.ToString());
            AlertHelper.DisplayMessage(HttpContext.Session, ViewBag);
            return View(new ControlsFilter() { AssetParameterTypeId = (Type == 0 ? null : Type), DisplayName = DisplayName });
        }
        [Route("Desk/Controls")]
        [HttpPost]
        public IActionResult Controls(ControlsFilter Model)
        {
            ViewBag.Data = Helpers.DBHelper.GetControls(AssetParameterTypeId: Model.AssetParameterTypeId, DisplayName: Model.DisplayName, DeptId:Model.DepartmentId, logger: _logger);
            Response.Cookies.Append("aptid", (Model.AssetParameterTypeId == null ? "" : Model.AssetParameterTypeId.ToString()));
            Response.Cookies.Append("apdn", (string.IsNullOrEmpty(Model.DisplayName) ? "" : Model.DisplayName));
            Response.Cookies.Append("apdid", (Model.DepartmentId == null ? "" : Model.DepartmentId.ToString()));
            return View(Model);
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
                return PartialView("ControlEdit", Model);
            }

            if (Model.Id == -1)
            {
                Helpers.DBHelper.CreateControl(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Контроль {Model.DisplayName} добавлен.");
            }
            else
            {
                Helpers.DBHelper.UpdateControl(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Контроль {Model.DisplayName} обновлен.");
            }
            return new JsonResult(0);  //return RedirectToAction("Controls");
        }

        [HttpGet]
        public IActionResult ControlDelete(int Id)
        {
            Helpers.DBHelper.DeleteControl(Id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Контроль удален.");
            return RedirectToAction("Controls");
        }

        [Obsolete]
        public IActionResult TOTypes()
        {
            ViewBag.TOTypes = Helpers.DBHelper.GetTOTypes(_logger);
            return View();
        }

        [HttpGet]
        [Obsolete]
        public IActionResult TOTypeCreate()
        {
            O2GEN.Models.TOType model = new();
            return PartialView("TOTypeEdit", model);
        }
        
        [HttpGet]
        [Obsolete]
        public IActionResult TOTypeEdit(int id)
        {
            var res = Helpers.DBHelper.GetTOType(id, _logger);

            if (res != null)
                return PartialView("TOTypeEdit", res);
            return View();
        }

        public IActionResult Departments()
        {
            ViewBag.Departments = Helpers.DBHelper.GetDepartments(-1, logger: _logger);
            AlertHelper.DisplayMessage(HttpContext.Session, ViewBag);
            return View();
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
            {
                Helpers.DBHelper.CreateDepartment(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Подразделение {Model.DisplayName} добавлено.");
            }
            else
            {
                Helpers.DBHelper.UpdateDepartment(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Подразделение {Model.DisplayName} обновлено.");
            }
            return new JsonResult(0);  //return RedirectToAction("Departments");
        }

        [HttpGet]
        public IActionResult DepartmentDelete(int Id)
        {
            Helpers.DBHelper.DeleteDepartment(Id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Подразделение удалено.");
            return RedirectToAction("Departments");
        }

        #region AJAX!
        [HttpGet]
        public JsonResult GetAssetsNodesJson(string ObjId)
        {
            int id = 0;
            if (int.TryParse(ObjId, out id))
                return new JsonResult(Helpers.DBHelper.GetSimpleAssets(null, id, _logger));
            else
                return new JsonResult(new List<Hierarchy>());
        }
        [HttpGet]
        public JsonResult GetAssetClassParametersJson(string ObjId)
        {
            int id = 0;
            if (int.TryParse(ObjId, out id))
                return new JsonResult(Helpers.DBHelper.GetAssetClassParameters(id, logger: _logger));
            else
                return new JsonResult(new List<AssetClassParameter>());
        }
        [HttpGet]
        public JsonResult GetControlsJson()
        {
            return new JsonResult(Helpers.DBHelper.GetControls(logger: _logger));
        }
        [HttpGet]
        public JsonResult GetAssetDetails(string ObjId, string DeptId)
        {
            int Id = 0;
            int Dept = 0;
            int.TryParse(ObjId, out Id);
            int.TryParse(DeptId, out Dept);
            return new JsonResult(Helpers.DBHelper.GetSimpleAssets((Id == 0 ? null : Id), (Dept == 0 ? null : Dept), _logger));
        }
        #endregion
    }
}
