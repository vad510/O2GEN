using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2GEN.Models;
using O2GEN.Models.HomeModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace O2GEN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ZprTakenModels zpr;
        private ControlValueReportListModel _controlValueReports;
        private ReportOnViewedTechPositionList _reportOnViewedTechPositionList;
        private ReportOnViewedTechPositionList _report;
        private DefectReportListModel _defectReport;

        public ReportModel ReportModel { get; set; }


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            GenerateMock();
          
        }

        #region Index
        public IActionResult Index()
        {
            ViewBag.ZRPCreated = Helpers.DBHelper.GetZRP(new DateTime(2021,3,23).AddMinutes(-30), new DateTime(2021, 4, 1).AddMinutes(30), _logger, (int)Helpers.ZRPStatus.Created);
            ViewBag.ZRPStarted = Helpers.DBHelper.GetZRP(new DateTime(2021,3,23).AddMinutes(-30), new DateTime(2021, 4, 1).AddMinutes(30), _logger, (int)Helpers.ZRPStatus.Started);
            ViewBag.ZRPEnded = Helpers.DBHelper.GetZRP(new DateTime(2021,3,23).AddMinutes(-30), new DateTime(2021, 4, 1).AddMinutes(30), _logger, (int)Helpers.ZRPStatus.Ended);
            return View();
        }
        #endregion

        [HttpGet]
        public IActionResult ZRPCreate()
        {
            ZRP model = new();
            return PartialView("ZRPEdit", model);
        }

        [HttpGet]
        public IActionResult ZRPEdit(int id)
        {
            var res = Helpers.DBHelper.GetZRP(id, _logger);
            if (res != null)
                return PartialView("ZRPEdit", res);
            return View();
        }
        [HttpPost]
        public IActionResult ZRPUpdate(ZRP Model)
        {
            if (Model.Id == -1)
                Helpers.DBHelper.CreateZRP(Model, User.Identity.Name, _logger);
            else
                Helpers.DBHelper.UpdateZRP(Model, User.Identity.Name, _logger);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult ZRPDelete(int Id)
        {
            Helpers.DBHelper.DeleteZRP(Id, User.Identity.Name, _logger);
            return RedirectToAction("Index");
        }

        public IActionResult ControlValueReports()
        {
            return View(_controlValueReports);
        }

        public IActionResult ReportsOnViewedTechPositions()
        {
            return View(_reportOnViewedTechPositionList);
        }

        public IActionResult NumberOfTechPosReports()
        {
            return View(_report);
        }

        public IActionResult DefectsReport()
        {
            return View(_defectReport);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void GenerateMock()
        {
            zpr = new();
            var _test = new ZprTakenModel
            {
                EquipmentType = "1",
                Status = "2",
                Type = "3",
                Bypass = "4",
                Bypasser = "5",
                Start = DateTime.Now,
                End = DateTime.Now
            };
            zpr.ZprTakenModelList.Add(_test);

            _controlValueReports = new ControlValueReportListModel();

            ControlValueReportModel cvrm = new ControlValueReportModel()
            {
                Control = "1",
                Node = "2",
                TechPosition = "3",
            };

            _controlValueReports.ControlValueReports.Add(cvrm);

            _reportOnViewedTechPositionList = new ReportOnViewedTechPositionList();
            _reportOnViewedTechPositionList.ReportOnViewedTechPositions.Add(new ReportOnViewedTechPosition
            {
                StationName = "1",
                FirstInterval = DateTime.Now,
                SecondInterval = DateTime.Now,
                Changes = 20
            });
            _report = new ReportOnViewedTechPositionList();

            _report.ReportOnViewedTechPositions.Add(new ReportOnViewedTechPosition
            {
                StationName = "1",
                FirstInterval = DateTime.Now,
                SecondInterval = DateTime.Now,
                Changes = 20
            });

            _defectReport = new DefectReportListModel();

            _defectReport.DefectReportModels.Add(new DefectReportModel()
            {
                TechPos = "1",
                Node = "2",
                Status = "3",
                DefectType = "4",
                Control = "5",
                Value = 6,
                DefectFoundedBy = "1",
                DefectProceededBy = "1",
                DateDetected = DateTime.Now,
                Description = "1"
            });

            ReportModel = new()
            {
                ControlValueReportModels = _controlValueReports,
                DefectReports = _defectReport,
                ZprTakenModels = zpr
            };
        }
    }
}
