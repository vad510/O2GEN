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
            return View(ReportModel);
        }
        #endregion

        [HttpGet]
        public IActionResult CreateZpr()
        {
            ZprTakenModel zprTakenModel = new();

            return PartialView("_ZprCreate", zprTakenModel);
        }

        [HttpGet]
        public IActionResult EditZpr(int id)
        {
            var res = ReportModel.ZprTakenModels.ZprTakenModelList.FirstOrDefault(x => x.Id == id);

            if (res != null)
                return PartialView("_ZprEdit", res);

            return View();
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
