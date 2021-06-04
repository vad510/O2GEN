using Microsoft.AspNetCore.Mvc;
using O2GEN.Models.DeskModels;

namespace O2GEN.Controllers
{
    public class DeskController : Controller
    {
        ObjectListModel obj = new();

        public IActionResult Objects()
        {
            ObjectListModel obj = new();

            for (var i = 0; i < 3; i++)
            {
                ObjectHeadModels headModel = new()
                {
                    ObjectName = "Head",
                    Description = "1",
                    Edit = "",
                    IndexMaximo = "1",
                    Status = "1",
                };

                for (var j = 0; j < 3; j++)
                {
                    ObjectModel objectModel = new ObjectModel
                    {
                        Name = "name",
                    };
                    headModel.ObjectModels.Add(objectModel);
                }

                obj.ObjectHeadModels.Add(headModel);
            }


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
            EmployeeCategoryList categoryList = new();

            for (var i = 0; i < 3; i++)
            {
                categoryList.EmployeeCategroys.Add(new EmployeeCategroyModel
                {
                    Name = i.ToString()
                });
            }

            return View(categoryList);
        }

        public IActionResult EmployeePosition()
        {
            EmployeeCategoryList categoryList = new();

            for (var i = 0; i < 3; i++)
            {
                categoryList.EmployeeCategroys.Add(new EmployeeCategroyModel
                {
                    Name = i.ToString()
                });
            }


            return View(categoryList);
        }

        public IActionResult ObjectClass()
        {
            EmployeeCategoryList categoryList = new();

            for (var i = 0; i < 3; i++)
            {
                categoryList.EmployeeCategroys.Add(new EmployeeCategroyModel
                {
                    Name = i.ToString()
                });
            }


            return View(categoryList);
        }

        public IActionResult Route()
        {
            RouteListModel rt = new();

            for (var i = 0; i < 3; i++)
            {
                rt.RouteModels.Add(new RouteModel
                {
                    Name = i.ToString(),
                    SubDivision = i.ToString()
                });
            }


            return View(rt);
        }

        public IActionResult Controls()
        {
            ControlListModel clm = new();

            for (var i = 0; i < 3; i++)
            {
                clm.ControlModels.Add(new ControlModel
                {
                    Name = i.ToString(),
                    Type = i.ToString()
                });
            }


            return View(clm);
        }

        public IActionResult TOTypes()
        {
            TOTypeListModel clm = new();

            for (var i = 0; i < 3; i++)
            {
                clm.TOTypes.Add(new TOTypeModel
                {
                    Name = i.ToString(),
                    VisibleName = i.ToString()
                });
            }


            return View(clm);
        }
    }
}
