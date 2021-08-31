using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2GEN.Models;
using O2GEN.Models.HomeModels;
using System;
using System.Collections.Generic;
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
        [Route("Home/Index")]
        [HttpGet]
        public IActionResult Index()
        {
            string FROMData = Request.Cookies["zrpfrom"];
            string TOData = Request.Cookies["zrpto"];
            string DepartmentIdData = Request.Cookies["zrpdid"];
            string DisplayName = Request.Cookies["zrpn"];

            long FromL = 0;
            long ToL = 0;
            int Did = 0;
            if (string.IsNullOrEmpty(FROMData) || !long.TryParse(FROMData, out FromL)) 
            {
                FROMData = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TOData) || !long.TryParse(TOData, out ToL))
            {
                TOData = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            int.TryParse(DepartmentIdData, out Did);
            DateTime From = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(FROMData)));
            DateTime To = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(TOData)));

            //ViewBag.ZRPCreated = Helpers.DBHelper.GetZRP(From, To, _logger, (int)Helpers.ZRPStatus.Created, (Did == 0 ? null : Did));
            ViewBag.ZRPInWork = Helpers.DBHelper.GetZRP(From, To, _logger, new int[] { (int)Helpers.ZRPStatus.Started, (int)Helpers.ZRPStatus.Created }, DisplayName, (Did == 0 ? null : Did));
            ViewBag.ZRPDone = Helpers.DBHelper.GetZRP(From, To, _logger, new int[] { (int)Helpers.ZRPStatus.Ended, (int)Helpers.ZRPStatus.Stoped }, DisplayName, (Did == 0 ? null : Did));
            Response.Cookies.Append("zrpfrom", FROMData);
            Response.Cookies.Append("zrpto", TOData);
            Response.Cookies.Append("zrpdid", string.IsNullOrEmpty(DepartmentIdData)?"": DepartmentIdData);
            Response.Cookies.Append("zrpn", (string.IsNullOrEmpty(DisplayName) ? "" : DisplayName));
            return View(new ZRPFilter() { From = Helpers.HandlingHelper.TicksFromNETToJS(From.Ticks).ToString(), To = Helpers.HandlingHelper.TicksFromNETToJS(To.Ticks).ToString(), DepartmentId = (Did == 0 ? null : Did), DisplayName = DisplayName });
        }
        [Route("Home/Index")]
        [HttpPost]
        public IActionResult Index(ZRPFilter Model)
        {
            DateTime From = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.From)));
            DateTime To = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.To)));
            //ViewBag.ZRPCreated = Helpers.DBHelper.GetZRP(From, To, _logger, (int)Helpers.ZRPStatus.Created, Model.DepartmentId);
            ViewBag.ZRPInWork = Helpers.DBHelper.GetZRP(From, To, _logger, new int[] { (int)Helpers.ZRPStatus.Started, (int)Helpers.ZRPStatus.Created }, Model.DisplayName, Model.DepartmentId);
            ViewBag.ZRPDone = Helpers.DBHelper.GetZRP(From, To, _logger, new int[] { (int)Helpers.ZRPStatus.Ended, (int)Helpers.ZRPStatus.Stoped }, Model.DisplayName, Model.DepartmentId);

            Response.Cookies.Append("zrpfrom", Model.From);
            Response.Cookies.Append("zrpto", Model.To);
            Response.Cookies.Append("zrpdid", (Model.DepartmentId == null?"": Model.DepartmentId.ToString()));
            return View(Model);
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
            var res = Helpers.DBHelper.GetZRP(id, User.Identity.Name, _logger);
            if (res != null)
                return PartialView("ZRPEdit", res);
            return View();
        }
        [HttpPost]
        public IActionResult ZRPUpdate(ZRP Model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ZRPEdit", Model);
            }
            if (Model.Id == -1)
            {
                Helpers.DBHelper.CreateZRP(Model, User.Identity.Name, _logger);
            }
            else
                Helpers.DBHelper.UpdateZRP(Model, User.Identity.Name, _logger);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult ZRPDelete(int Id)
        {
            Helpers.DBHelper.DeleteZRP(Id, User.Identity.Name, _logger);
            return RedirectToAction("Index", "Home");
        }

        [Route("Home/ControlValueReports")]
        [HttpGet]
        public IActionResult ControlValueReports()
        {
            string FROMData = Request.Cookies["RepCVFrom"];
            string TOData = Request.Cookies["RepCVTo"];
            string DIdData = Request.Cookies["RepCVDId"];
            string APSIdData = Request.Cookies["RepCVAPSId"];
            string APIdData = Request.Cookies["RepCVAPId"];

            long FROMParsed = 0;
            long TOParsed = 0;
            int DIdParsed = 0;
            int APSIdParsed = 0;
            int APIdParsed = 0;
            if (string.IsNullOrEmpty(FROMData) || !long.TryParse(FROMData, out FROMParsed))
            {
                FROMData = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TOData) || !long.TryParse(TOData, out TOParsed))
            {
                TOData = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            int.TryParse(DIdData, out DIdParsed);
            int.TryParse(APSIdData, out APSIdParsed);
            int.TryParse(APIdData, out APIdParsed);
            DateTime From = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(FROMData)));
            DateTime To = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(TOData)));

            ViewBag.Data = null;
            Response.Cookies.Append("RepCVFrom", FROMData);
            Response.Cookies.Append("RepCVTo", TOData);
            Response.Cookies.Append("RepCVDId", string.IsNullOrEmpty(DIdData) ? "" : DIdData);
            Response.Cookies.Append("RepCVAPSId", string.IsNullOrEmpty(APSIdData) ? "" : APSIdData);
            Response.Cookies.Append("RepCVAPId", string.IsNullOrEmpty(APIdData) ? "" : APIdData);
            return View(new ControlValueReportFilter()
            {
                From = Helpers.HandlingHelper.TicksFromNETToJS(From.Ticks).ToString(),
                To = Helpers.HandlingHelper.TicksFromNETToJS(To.Ticks).ToString(),
                DepartmentId = (DIdParsed == 0 ? null : DIdParsed),
                AssetParameterSetId = (APSIdParsed == 0 ? null : APSIdParsed),
                AssetId = (APIdParsed == 0 ? null : APIdParsed)
            });
        }
        [Route("Home/ControlValueReports")]
        [HttpPost]
        public IActionResult ControlValueReports(ControlValueReportFilter Model)
        {
            DateTime From = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.From)));
            DateTime To = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.To)));
            ViewBag.Data = Helpers.DBHelper.GetCVR(From, To, (long)Model.DepartmentId, (long)Model.AssetId, logger: _logger);

            Response.Cookies.Append("RepCVFrom", Model.From);
            Response.Cookies.Append("RepCVTo", Model.To);
            Response.Cookies.Append("RepCVDId", (Model.DepartmentId == null ? "" : Model.DepartmentId.ToString()));
            Response.Cookies.Append("RepCVAPSId", (Model.AssetParameterSetId == null ? "" : Model.AssetParameterSetId.ToString()));
            Response.Cookies.Append("RepCVAPId", (Model.AssetId == null ? "" : Model.AssetId.ToString()));
            return View(Model);
        }

        [Route("Home/ReportsOnViewedTechPositions")]
        [HttpGet]
        public IActionResult ReportsOnViewedTechPositions()
        {
            string FROM1Data = Request.Cookies["RepVAFrom1"];
            string TO1Data = Request.Cookies["RepVATo1"];
            string FROM2Data = Request.Cookies["RepVAFrom2"];
            string TO2Data = Request.Cookies["RepVATo2"];

            long FROM1Parsed = 0;
            long TO1Parsed = 0;
            long FROM2Parsed = 0;
            long TO2Parsed = 0;
            if (string.IsNullOrEmpty(FROM1Data) || !long.TryParse(FROM1Data, out FROM1Parsed))
            {
                FROM1Data = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TO1Data) || !long.TryParse(TO1Data, out TO1Parsed))
            {
                TO1Data = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            if (string.IsNullOrEmpty(FROM2Data) || !long.TryParse(FROM2Data, out FROM2Parsed))
            {
                FROM2Data = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TO2Data) || !long.TryParse(TO2Data, out TO2Parsed))
            {
                TO2Data = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            DateTime From1 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(FROM1Data)));
            DateTime To1 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(TO1Data)));
            DateTime From2 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(FROM2Data)));
            DateTime To2 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(TO2Data)));

            ViewBag.Data = null;
            Response.Cookies.Append("RepVAFrom1", FROM1Data);
            Response.Cookies.Append("RepVATo1", TO1Data);
            Response.Cookies.Append("RepVAFrom2", FROM2Data);
            Response.Cookies.Append("RepVATo2", TO2Data);
            return View(new AssetsReportFilter()
            {
                From1 = Helpers.HandlingHelper.TicksFromNETToJS(From1.Ticks).ToString(),
                To1 = Helpers.HandlingHelper.TicksFromNETToJS(To1.Ticks).ToString(),
                From2 = Helpers.HandlingHelper.TicksFromNETToJS(From2.Ticks).ToString(),
                To2 = Helpers.HandlingHelper.TicksFromNETToJS(To2.Ticks).ToString()
            });
        }
        [Route("Home/ReportsOnViewedTechPositions")]
        [HttpPost]
        public IActionResult ReportsOnViewedTechPositions(AssetsReportFilter Model)
        {
            DateTime From1 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.From1)));
            DateTime To1 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.To1)));
            DateTime From2 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.From2)));
            DateTime To2 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.To2)));

            ViewBag.Data = new AssetsReportMerge(Helpers.DBHelper.GetDepartments(ClearList: true, logger: _logger), Helpers.DBHelper.GetVA(From1, To1, _logger), Helpers.DBHelper.GetVA(From2, To2, _logger));

            Response.Cookies.Append("RepVAFrom1", Model.From1);
            Response.Cookies.Append("RepVATo1", Model.To1);
            Response.Cookies.Append("RepVAFrom2", Model.From2);
            Response.Cookies.Append("RepVATo2", Model.To2);
            return View(Model);
        }

        [Route("Home/NumberOfTechPosReports")]
        [HttpGet]
        public IActionResult NumberOfTechPosReports()
        {
            string FROM1Data = Request.Cookies["RepNAFrom1"];
            string TO1Data = Request.Cookies["RepNATo1"];
            string FROM2Data = Request.Cookies["RepNAFrom2"];
            string TO2Data = Request.Cookies["RepNATo2"];

            long FROM1Parsed = 0;
            long TO1Parsed = 0;
            long FROM2Parsed = 0;
            long TO2Parsed = 0;
            if (string.IsNullOrEmpty(FROM1Data) || !long.TryParse(FROM1Data, out FROM1Parsed))
            {
                FROM1Data = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TO1Data) || !long.TryParse(TO1Data, out TO1Parsed))
            {
                TO1Data = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            if (string.IsNullOrEmpty(FROM2Data) || !long.TryParse(FROM2Data, out FROM2Parsed))
            {
                FROM2Data = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TO2Data) || !long.TryParse(TO2Data, out TO2Parsed))
            {
                TO2Data = Helpers.HandlingHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            DateTime From1 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(FROM1Data)));
            DateTime To1 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(TO1Data)));
            DateTime From2 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(FROM2Data)));
            DateTime To2 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(TO2Data)));

            ViewBag.Data = null;
            Response.Cookies.Append("RepNAFrom1", FROM1Data);
            Response.Cookies.Append("RepNATo1", TO1Data);
            Response.Cookies.Append("RepNAFrom2", FROM2Data);
            Response.Cookies.Append("RepNATo2", TO2Data);
            return View(new AssetsReportFilter()
            {
                From1 = Helpers.HandlingHelper.TicksFromNETToJS(From1.Ticks).ToString(),
                To1 = Helpers.HandlingHelper.TicksFromNETToJS(To1.Ticks).ToString(),
                From2 = Helpers.HandlingHelper.TicksFromNETToJS(From2.Ticks).ToString(),
                To2 = Helpers.HandlingHelper.TicksFromNETToJS(To2.Ticks).ToString()
            });
        }
        [Route("Home/NumberOfTechPosReports")]
        [HttpPost]
        public IActionResult NumberOfTechPosReports(AssetsReportFilter Model)
        {
            DateTime From1 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.From1)));
            DateTime To1 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.To1)));
            DateTime From2 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.From2)));
            DateTime To2 = new DateTime().AddTicks(Helpers.HandlingHelper.TicksFromJSToNET(long.Parse(Model.To2)));

            ViewBag.Data = new AssetsReportMerge(Helpers.DBHelper.GetDepartments(ClearList: true, logger: _logger), Helpers.DBHelper.GetNA(From1, To1, _logger), Helpers.DBHelper.GetNA(From2, To2, _logger));

            Response.Cookies.Append("RepNAFrom1", Model.From1);
            Response.Cookies.Append("RepNATo1", Model.To1);
            Response.Cookies.Append("RepNAFrom2", Model.From2);
            Response.Cookies.Append("RepNATo2", Model.To2);
            return View(Model);
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


        [HttpGet]
        public IActionResult Start()
        {
            return View();
        }

        #region AJAX!
        [HttpGet]
        public JsonResult GetResourcesByDepartmentJson(string ObjId)
        {
            int id = 0;
            if (int.TryParse(ObjId, out id))
                return new JsonResult(Helpers.DBHelper.GetResources(DeptId:id, logger:_logger));
            else 
                return new JsonResult( new List<Resource>());
        }
        [HttpGet]
        public JsonResult GetAssetParameterSetByDepartmentJson(string ObjId)
        {
            int id = 0;
            if (int.TryParse(ObjId, out id))
                return new JsonResult(Helpers.DBHelper.GetAssetParameterSets(_logger,DeptID:id));
            else
                return new JsonResult(new List<AssetParameterSet>());
        }
        [HttpGet]
        public JsonResult GetRouteDetailsJson(string ObjId)
        {
            int id = 0;
            if (int.TryParse(ObjId, out id))
                return new JsonResult(Helpers.DBHelper.GetAssetParameterSetDetails(id, _logger));
            else
                return new JsonResult(new List<Hierarchy>());
        }
        [HttpGet]
        public JsonResult GetAssetParameterSets(string ObjId)
        {
            int id = 0;
            if (int.TryParse(ObjId, out id))
                return new JsonResult(Helpers.DBHelper.GetAssetParameterSets( _logger, id));
            else
                return new JsonResult(new List<AssetParameterSet>());
        }
        [HttpGet]
        public JsonResult GetAssets(string ObjId)
        {
            int id = 0;
            if (int.TryParse(ObjId, out id))
                return new JsonResult(Helpers.DBHelper.GetAssetsFromAPS(id, _logger));
            else
                return new JsonResult(new List<Asset>());
        }
        #endregion
    }
}
