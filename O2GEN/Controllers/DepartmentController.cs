using Microsoft.AspNetCore.Mvc;
using O2GEN.Models.EmployeeModels;

namespace O2GEN.Controllers
{
    public class DepartmentController : Controller
    {
        public DepartmentController()
        {

        }

        public IActionResult Index()
        {
            DepartmentListModel dps = new();

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

            return View(dps);
        }
    }
}
