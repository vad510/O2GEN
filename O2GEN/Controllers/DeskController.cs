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
            //ObjectListModel obj = new();

            //for (var i = 0; i < 3; i++)
            //{
            //    ObjectHeadModels headModel = new()
            //    {
            //        ObjectName = "Head",
            //        Description = "1",
            //        Edit = "",
            //        IndexMaximo = "1",
            //        Status = "1",
            //    };

            //    for (var j = 0; j < 3; j++)
            //    {
            //        ObjectModel objectModel = new ObjectModel
            //        {
            //            Name = "name",
            //        };
            //        headModel.ObjectModels.Add(objectModel);
            //    }

            //    obj.ObjectHeadModels.Add(headModel);
            //}


            ViewBag.Objects = Helpers.DBHelper.GetAssets(_logger);
            return View(obj);
        }

        [HttpGet]
        public IActionResult CreateObject()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditObject()
        {
            return View();
        }


        public IActionResult EmployeeCategory()
        {
            //EmployeeCategoryList categoryList = new();

            //for (var i = 0; i < 3; i++)
            //{
            //    categoryList.EmployeeCategroys.Add(new EmployeeCategroyModel
            //    {
            //        Name = i.ToString()
            //    });
            //}


            ViewBag.EmployeeCategories = Helpers.DBHelper.GetPersonCategories(_logger);

            return View();
        }

        public IActionResult EmployeePosition()
        {
            //EmployeeCategoryList categoryList = new();

            //for (var i = 0; i < 3; i++)
            //{
            //    categoryList.EmployeeCategroys.Add(new EmployeeCategroyModel
            //    {
            //        Name = i.ToString()
            //    });
            //}


            ViewBag.EmployeePositions = Helpers.DBHelper.GetPersonPositions(_logger);
            return View();
        }

        public IActionResult ObjectClass()
        {
            //EmployeeCategoryList categoryList = new();

            //for (var i = 0; i < 3; i++)
            //{
            //    categoryList.EmployeeCategroys.Add(new EmployeeCategroyModel
            //    {
            //        Name = i.ToString()
            //    });
            //}


            ViewBag.ObjectClasses = Helpers.DBHelper.GetPersonPositions(_logger);
            return View();
        }

        public IActionResult Route()
        {
            //RouteListModel rt = new();

            //for (var i = 0; i < 3; i++)
            //{
            //    rt.RouteModels.Add(new RouteModel
            //    {
            //        Name = i.ToString(),
            //        SubDivision = i.ToString()
            //    });
            //}


            ViewBag.Routes = Helpers.DBHelper.GetAssetParameterSets(_logger);
            return View();
        }

        public IActionResult Controls()
        {
            //ControlListModel clm = new();

            //for (var i = 0; i < 3; i++)
            //{
            //    clm.ControlModels.Add(new ControlModel
            //    {
            //        Name = i.ToString(),
            //        Type = i.ToString()
            //    });
            //}


            ViewBag.Controls = Helpers.DBHelper.GetControls(_logger);
            return View();
        }

        public IActionResult TOTypes()
        {
            //TOTypeListModel clm = new();

            //for (var i = 0; i < 3; i++)
            //{
            //    clm.TOTypes.Add(new TOTypeModel
            //    {
            //        Name = i.ToString(),
            //        VisibleName = i.ToString()
            //    });
            //}


            ViewBag.TOTypes = Helpers.DBHelper.GetTOTypes(_logger);
            return View();
        }
    }
}
