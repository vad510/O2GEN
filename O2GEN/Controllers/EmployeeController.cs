using Microsoft.AspNetCore.Mvc;
using O2GEN.Models.EmployeeModels;
using O2GEN.Models.HomeModels;
using System.Linq;

namespace O2GEN.Controllers
{
    public class EmployeeController : Controller
    {
        private EmployeeListModel _employeeListModels;
        private DepartmentListModel dps = new();

        public EmployeeController()
        {
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

        public IActionResult Index()
        {
            return View(_employeeListModels);
        }

        public IActionResult RoleList()
        {
            RoleListModels rm = new();

            rm.RoleModels.Add(new RoleModel
            {
                RoleName = "1",
                VisibleRoleName = "1",
                Edit = ""
            });

            return View(rm);
        }

        public IActionResult Resources()
        {
            ResourcesListModels rs = new();

            rs.ResourcesModels.Add(new ResourcesModel
            {
                VisibleName = "1",
                Edit = ""
            });

            return View(rs);
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public IActionResult Department()
        {
            return View(dps);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">element clicked</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditDepartment(int id)
        {
            return PartialView("_DepartmentEdit", dps.DepartmentHeadModels.FirstOrDefault());
        }
    }
}
