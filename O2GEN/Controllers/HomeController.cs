using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2GEN.Authorization;
using O2GEN.Helpers;
using O2GEN.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace O2GEN.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        #region Index
        [Route("Home/Index")]
        [HttpGet]
        public IActionResult Index()
        {
            string 
            FROMData = Request.Cookies["zrpfrom"],
            TOData = Request.Cookies["zrpto"],
            DepartmentIdData = Request.Cookies["zrpdid"],
            DisplayName = Request.Cookies["zrpn"];

            long Did = 0;
            DateTime From, To;
            if (!DateTime.TryParse(FROMData, out From))
            {
                From = DateTime.Now.Date.AddDays(-2);
            }
            if (!DateTime.TryParse(TOData, out To))
            {
                To = DateTime.Now;
            }
            FROMData = From.ToString("yyyy-MM-ddTHH:mm");
            TOData = To.ToString("yyyy-MM-ddTHH:mm");

            ViewBag.ZRPInWork = Helpers.DBHelper.GetZRP(From, To, _logger, new int[] { (int)Helpers.ZRPStatus.Started, (int)Helpers.ZRPStatus.Created }, DisplayName, (Did == 0 ? null : Did), ((Credentials)HttpContext.Items["User"]).DeptId);
            ViewBag.ZRPDone = Helpers.DBHelper.GetZRP(From, To, _logger, new int[] { (int)Helpers.ZRPStatus.Ended, (int)Helpers.ZRPStatus.Stoped }, DisplayName, (Did == 0 ? null : Did), ((Credentials)HttpContext.Items["User"]).DeptId);
            Response.Cookies.Append("zrpfrom", FROMData);
            Response.Cookies.Append("zrpto", TOData);
            Response.Cookies.Append("zrpdid", string.IsNullOrEmpty(DepartmentIdData)?"" : DepartmentIdData);
            Response.Cookies.Append("zrpn", (string.IsNullOrEmpty(DisplayName) ? "" : DisplayName));
            AlertHelper.DisplayMessage(HttpContext.Session, ViewBag);
            return View(new ZRPFilter()
            {
                From = FROMData,
                To = TOData,
                DepartmentId = (Did == 0 ? null : Did), DisplayName = DisplayName 
            });
        }
        [Route("Home/Index")]
        [HttpPost]
        public IActionResult Index(ZRPFilter Model)
        {
            DateTime From = DateTime.Parse(Model.From);
            DateTime To = DateTime.Parse(Model.To);
            ViewBag.ZRPInWork = Helpers.DBHelper.GetZRP(From, To, _logger, new int[] { (int)Helpers.ZRPStatus.Started, (int)Helpers.ZRPStatus.Created }, Model.DisplayName, Model.DepartmentId, ((Credentials)HttpContext.Items["User"]).DeptId);
            ViewBag.ZRPDone = Helpers.DBHelper.GetZRP(From, To, _logger, new int[] { (int)Helpers.ZRPStatus.Ended, (int)Helpers.ZRPStatus.Stoped }, Model.DisplayName, Model.DepartmentId, ((Credentials)HttpContext.Items["User"]).DeptId);

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
            var res = Helpers.DBHelper.GetZRP(id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            if (res != null)
                return PartialView("ZRPEdit", res);
            return View();
        }
        [HttpPost]
        [DisableRequestSizeLimit]
        public IActionResult ZRPUpdate(ZRP Model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ZRPEdit", Model);
            }
            if (Model.Id == -1)
            {
                Helpers.DBHelper.CreateZRP(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Обход {Model.RouteName} добавлен.");
            }
            else
            {
                Helpers.DBHelper.UpdateZRP(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
                AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Обход {Model.RouteName} обновлен.");
            }
            return new JsonResult(0);  //return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult ZRPDelete(int Id)
        {
            Helpers.DBHelper.DeleteZRP(Id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Обход удален.");
            return RedirectToAction("Index", "Home");
        }

        [Route("Home/ControlValueReports")]
        [HttpGet]
        public IActionResult ControlValueReports()
        {
            string FROMData = Request.Cookies["RepCVFrom"],
            TOData = Request.Cookies["RepCVTo"],
            DIdData = Request.Cookies["RepCVDId"],
            APSIdData = Request.Cookies["RepCVAPSId"],
            APIdData = Request.Cookies["RepCVAPId"];

            long 
            DIdParsed = 0,
            APSIdParsed = 0,
            APIdParsed = 0;
            DateTime From, To;
            if (!DateTime.TryParse(FROMData, out From))
            {
                From = DateTime.Now.Date.AddDays(-2);
            }
            if (!DateTime.TryParse(TOData, out To))
            {
                To = DateTime.Now;
            }
            FROMData = From.ToString("yyyy-MM-ddTHH:mm");
            TOData = To.ToString("yyyy-MM-ddTHH:mm");

            if (!long.TryParse(DIdData, out DIdParsed)) DIdParsed = (int)((Credentials)HttpContext.Items["User"]).DeptId;
            long.TryParse(APSIdData, out APSIdParsed);
            long.TryParse(APIdData, out APIdParsed);
            ViewBag.Data = null;
            Response.Cookies.Append("RepCVFrom", FROMData);
            Response.Cookies.Append("RepCVTo", TOData);
            Response.Cookies.Append("RepCVDId", string.IsNullOrEmpty(DIdData) ? "" : DIdData);
            Response.Cookies.Append("RepCVAPSId", string.IsNullOrEmpty(APSIdData) ? "" : APSIdData);
            Response.Cookies.Append("RepCVAPId", string.IsNullOrEmpty(APIdData) ? "" : APIdData);
            return View(new ControlValueReportFilter()
            {
                From = FROMData,
                To = TOData,
                DepartmentId = (DIdParsed == 0 ? null : DIdParsed),
                AssetParameterSetId = (APSIdParsed == 0 ? null : APSIdParsed),
                AssetId = (APIdParsed == 0 ? null : APIdParsed)
            });
            
        }
        [Route("Home/ControlValueReports")]
        [HttpPost]
        public IActionResult ControlValueReports(ControlValueReportFilter Model)
        {
            
            string message = "";
            if (Model.DepartmentId == null)
            {
                message += "Не указано подразделение. " + Environment.NewLine;
            }
            if (Model.AssetParameterSetId == null)
            {
                if (!string.IsNullOrEmpty(message)) message += Environment.NewLine;
                message += "Не указан маршрут. " + Environment.NewLine;
            }
            if(Model.AssetId == null)
            {
                if (!string.IsNullOrEmpty(message)) message += Environment.NewLine;
                message += "Не указана тех. позиция.  ";
            }
            if (!string.IsNullOrEmpty(message))
            {
                AlertHelper.DisplayMessage(ViewBag, AlertType.Warning, message);
                return View(Model);
            }
            DateTime From = DateTime.Parse(Model.From);
            DateTime To = DateTime.Parse(Model.To);
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
            string
            FROMData1 = Request.Cookies["RepVAFrom1"],
            TOData1 = Request.Cookies["RepVATo1"],
            FROMData2 = Request.Cookies["RepVAFrom2"],
            TOData2 = Request.Cookies["RepVATo2"];

            DateTime From1, To1, From2, To2;
            if (!DateTime.TryParse(FROMData1, out From1))
            {
                From1 = DateTime.Now.Date.AddDays(-2);
            }
            if (!DateTime.TryParse(TOData1, out To1))
            {
                To1 = DateTime.Now;
            }
            if (!DateTime.TryParse(FROMData2, out From2))
            {
                From2 = DateTime.Now.Date.AddDays(-2);
            }
            if (!DateTime.TryParse(TOData2, out To2))
            {
                To2 = DateTime.Now;
            }
            FROMData1 = From1.ToString("yyyy-MM-ddTHH:mm");
            TOData1 = To1.ToString("yyyy-MM-ddTHH:mm");
            FROMData2 = From2.ToString("yyyy-MM-ddTHH:mm");
            TOData2 = To2.ToString("yyyy-MM-ddTHH:mm");

            ViewBag.Data = null;
            Response.Cookies.Append("RepVAFrom1", FROMData1);
            Response.Cookies.Append("RepVATo1", TOData1);
            Response.Cookies.Append("RepVAFrom2", FROMData2);
            Response.Cookies.Append("RepVATo2", TOData2);
            return View(new AssetsReportFilter()
            {
                From1 = FROMData1,
                To1 = TOData1,
                From2 = FROMData2,
                To2 = TOData2
            });
        }
        [Route("Home/ReportsOnViewedTechPositions")]
        [HttpPost]
        public IActionResult ReportsOnViewedTechPositions(AssetsReportFilter Model)
        {
            DateTime From1 = DateTime.Parse(Model.From1);
            DateTime To1 = DateTime.Parse(Model.To1);
            DateTime From2 = DateTime.Parse(Model.From2);
            DateTime To2 = DateTime.Parse(Model.To2);

            ViewBag.Data = new AssetsReportMerge(Helpers.DBHelper.GetChildDepartments(((Credentials)HttpContext.Items["User"]).DeptId, logger: _logger), Helpers.DBHelper.GetVA(From1, To1, _logger), Helpers.DBHelper.GetVA(From2, To2, _logger));

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
            string FROMData1 = Request.Cookies["RepNAFrom1"];
            string TOData1 = Request.Cookies["RepNATo1"];
            string FROMData2 = Request.Cookies["RepNAFrom2"];
            string TOData2 = Request.Cookies["RepNATo2"];

            DateTime From1, To1, From2, To2;
            if (!DateTime.TryParse(FROMData1, out From1))
            {
                From1 = DateTime.Now.Date.AddDays(-2);
            }
            if (!DateTime.TryParse(TOData1, out To1))
            {
                To1 = DateTime.Now;
            }
            if (!DateTime.TryParse(FROMData2, out From2))
            {
                From2 = DateTime.Now.Date.AddDays(-2);
            }
            if (!DateTime.TryParse(TOData2, out To2))
            {
                To2 = DateTime.Now;
            }
            FROMData1 = From1.ToString("yyyy-MM-ddTHH:mm");
            TOData1 = To1.ToString("yyyy-MM-ddTHH:mm");
            FROMData2 = From2.ToString("yyyy-MM-ddTHH:mm");
            TOData2 = To2.ToString("yyyy-MM-ddTHH:mm");

            ViewBag.Data = null;
            Response.Cookies.Append("RepNAFrom1", FROMData1);
            Response.Cookies.Append("RepNATo1", TOData1);
            Response.Cookies.Append("RepNAFrom2", FROMData2);
            Response.Cookies.Append("RepNATo2", TOData2);
            return View(new AssetsReportFilter()
            {
                From1 = FROMData1,
                To1 = TOData1,
                From2 = FROMData2,
                To2 = TOData2
            });
        }
        [Route("Home/NumberOfTechPosReports")]
        [HttpPost]
        public IActionResult NumberOfTechPosReports(AssetsReportFilter Model)
        {
            DateTime From1 = DateTime.Parse(Model.From1);
            DateTime To1 = DateTime.Parse(Model.To1);
            DateTime From2 = DateTime.Parse(Model.From2);
            DateTime To2 = DateTime.Parse(Model.To2);

            ViewBag.Data = new AssetsReportMerge(Helpers.DBHelper.GetChildDepartments(((Credentials)HttpContext.Items["User"]).DeptId, logger: _logger), Helpers.DBHelper.GetNA(From1, To1, _logger), Helpers.DBHelper.GetNA(From2, To2, _logger));

            Response.Cookies.Append("RepNAFrom1", Model.From1);
            Response.Cookies.Append("RepNATo1", Model.To1);
            Response.Cookies.Append("RepNAFrom2", Model.From2);
            Response.Cookies.Append("RepNATo2", Model.To2);
            return View(Model);
        }

        public IActionResult DefectsReport()
        {
            return View();
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

        [Route("Home/ControlTrends")]
        [HttpGet]
        public IActionResult ControlTrends()
        {
            string FROMData = Request.Cookies["ctfrom"],
            TOData = Request.Cookies["ctto"],
            DIdData = Request.Cookies["ctdid"],
            APSIdData = Request.Cookies["ctapsid"],
            AIdData = Request.Cookies["ctaid"],
            ACIdData = Request.Cookies["ctacid"],
            APIdData = Request.Cookies["ctapid"];

            long 
            DId = 0,
            APSId = 0,
            AId = 0,
            ACId = 0,
            APId = 0;
            DateTime From, To;
            if (!DateTime.TryParse(FROMData, out From))
            {
                From = DateTime.Now.Date.AddDays(-2);
            }
            if (!DateTime.TryParse(TOData, out To))
            {
                To = DateTime.Now;
            }
            FROMData = From.ToString("yyyy-MM-ddTHH:mm");
            TOData = To.ToString("yyyy-MM-ddTHH:mm");

            if(!long.TryParse(DIdData, out DId)) DId = (int)((Credentials)HttpContext.Items["User"]).DeptId;
            long.TryParse(APSIdData, out APSId);
            long.TryParse(AIdData, out AId);
            long.TryParse(ACIdData, out ACId);
            long.TryParse(APIdData, out APId);

            Response.Cookies.Append("ctfrom", FROMData);
            Response.Cookies.Append("ctto", TOData);
            Response.Cookies.Append("ctdid", string.IsNullOrEmpty(DIdData) ? "" : DIdData);
            Response.Cookies.Append("ctapsid", string.IsNullOrEmpty(APSIdData) ? "" : APSIdData);
            Response.Cookies.Append("ctaid", string.IsNullOrEmpty(AIdData) ? "" : AIdData);
            Response.Cookies.Append("ctacid", string.IsNullOrEmpty(ACIdData) ? "" : ACIdData);
            Response.Cookies.Append("ctapid", string.IsNullOrEmpty(APIdData) ? "" : APIdData);
            return View(
                new ControlTrendsFilter() 
                { 
                    From = FROMData, 
                    To = TOData, 
                    DepartmentId = (DId == 0? null:DId),
                    AssetParameterSetId = (APSId == 0 ? null : APSId),
                    AssetId = (AId == 0 ? null : AId),
                    AssetChildId = (ACId == 0 ? null : ACId),
                    AssetParameterId = (APId == 0 ? null : APId)
                });
        }
        [Route("Home/ControlTrends")]
        [HttpPost]
        public IActionResult ControlTrends(ControlTrendsFilter Model)
        {
            if(string.IsNullOrEmpty(Model.From) ||
                string.IsNullOrEmpty(Model.To) ||
                Model.DepartmentId is null ||
                Model.AssetParameterSetId is null ||
                Model.AssetId is null ||
                Model.AssetChildId is null ||
                Model.AssetParameterId is null )
            {
                ViewBag.Message = "Данные для указанного фильтра не найдены.";
            }
            else
            {
                DateTime From = DateTime.Parse(Model.From);
                DateTime To = DateTime.Parse(Model.To);
                List<ControlStatistic> data = Helpers.DBHelper.GetControlStatistic(Model.DepartmentId, Model.AssetParameterSetId, Model.AssetId, Model.AssetChildId, Model.AssetParameterId, From, To, _logger);

                ViewBag.Data = data;
                Control control = Helpers.DBHelper.GetControl((long)Model.AssetParameterId, _logger);
                ViewBag.Control = control;
                ViewBag.Maximum = "0";
                ViewBag.Minimum = "0";
                if (!string.IsNullOrEmpty(control.ValueBottom3) &&
                    !string.IsNullOrEmpty(control.ValueTop3) &&
                    !string.IsNullOrEmpty(control.ValueBottom2) &&
                    !string.IsNullOrEmpty(control.ValueTop2) &&
                    !string.IsNullOrEmpty(control.ValueBottom1) &&
                    !string.IsNullOrEmpty(control.ValueTop1))
                {
                    List<double> ustavki = new List<double>();
                    ustavki.Add(double.Parse(control.ValueBottom3, CultureInfo.InvariantCulture));
                    ustavki.Add(double.Parse(control.ValueBottom2, CultureInfo.InvariantCulture));
                    ustavki.Add(double.Parse(control.ValueBottom1, CultureInfo.InvariantCulture));
                    ustavki.Add(double.Parse(control.ValueTop3, CultureInfo.InvariantCulture));
                    ustavki.Add(double.Parse(control.ValueTop2, CultureInfo.InvariantCulture));
                    ustavki.Add(double.Parse(control.ValueTop1, CultureInfo.InvariantCulture));
                    if (ustavki.Count > 0)
                    {
                        ViewBag.Maximum = ustavki.Max().ToString().Replace(",", ".");
                        ViewBag.Minimum = ustavki.Min().ToString().Replace(",", ".");
                    }
                }
                if (data != null && data.Count > 0)
                {
                    double? tmp = data.Max(x => x.y) + data.Max(x => x.y) * 0.1;
                    ViewBag.Maximum = (tmp > double.Parse(ViewBag.Maximum, CultureInfo.InvariantCulture)) ? tmp.ToString().Replace(",", ".") : ViewBag.Maximum;
                    tmp = data.Min(x => x.y) - data.Min(x => x.y) * 0.1;
                    ViewBag.Minimum = (tmp < double.Parse(ViewBag.Minimum, CultureInfo.InvariantCulture)) ? tmp.ToString().Replace(",", ".") : ViewBag.Minimum;
                }
                else
                {
                    ViewBag.Message = "Данные для указанного фильтра не найдены.";
                }
            }

            Response.Cookies.Append("ctfrom", Model.From);
            Response.Cookies.Append("ctpto", Model.To);
            Response.Cookies.Append("ctdid", (Model.DepartmentId == null ? "" : Model.DepartmentId.ToString()));
            Response.Cookies.Append("ctapsid", (Model.AssetParameterSetId == null ? "" : Model.AssetParameterSetId.ToString()));
            Response.Cookies.Append("ctaid", (Model.AssetId == null ? "" : Model.AssetId.ToString()));
            Response.Cookies.Append("ctacid", (Model.AssetChildId == null ? "" : Model.AssetChildId.ToString()));
            Response.Cookies.Append("ctapid", (Model.AssetParameterId == null ? "" : Model.AssetParameterId.ToString()));
            return View(Model);
        }

        [Route("Home/Statistics")]
        [HttpGet]
        public IActionResult Statistics()
        {
            string FROMData = Request.Cookies["statfrom"];
            string TOData = Request.Cookies["statto"];
            string DepartmentIdData = Request.Cookies["statdid"];

            long Did = 0;

            DateTime From, To;
            if (!DateTime.TryParse(FROMData, out From))
            {
                From = DateTime.Now.Date.AddDays(-2);
            }
            if (!DateTime.TryParse(TOData, out To))
            {
                To = DateTime.Now;
            }
            FROMData = From.ToString("yyyy-MM-ddTHH:mm");
            TOData = To.ToString("yyyy-MM-ddTHH:mm");

            //Если это РТЭЦ2 для него особый отчет.
            #warning переделать на динамическое определение.
            if (Did == 2 ||
                Did == 3 ||
                Did == 4 ||
                Did == 5)
            {
                ViewBag.Data = Helpers.DBHelper.GetStatisticsRTEC2(From, To, Did, _logger);
            }
            else
                ViewBag.Data = Helpers.DBHelper.GetStatistics(From, To, (Did == 0 ? null : Did), _logger);
            Response.Cookies.Append("statfrom", FROMData);
            Response.Cookies.Append("statto", TOData);
            Response.Cookies.Append("statdid", string.IsNullOrEmpty(DepartmentIdData) ? "" : DepartmentIdData);
            return View(new StatisticsFilter() 
            { 
                From = FROMData, 
                To = TOData, 
                DepartmentId = (Did == 0 ? null : Did) 
            });
        }
        [Route("Home/Statistics")]
        [HttpPost]
        public IActionResult Statistics(StatisticsFilter Model)
        {
            DateTime From = DateTime.Parse(Model.From);
            DateTime To = DateTime.Parse(Model.To);
            //Если это РТЭЦ2 для него особый отчет.
            #warning переделать на динамическое определение.
            if(Model.DepartmentId == 2 ||
                Model.DepartmentId == 3 ||
                Model.DepartmentId == 4 ||
                Model.DepartmentId == 5)
            {
                ViewBag.Data = Helpers.DBHelper.GetStatisticsRTEC2(From, To, Model.DepartmentId, _logger);
            }
            else ViewBag.Data = Helpers.DBHelper.GetStatistics(From, To, Model.DepartmentId, _logger);

            Response.Cookies.Append("statfrom", Model.From);
            Response.Cookies.Append("statto", Model.To);
            Response.Cookies.Append("statdid", (Model.DepartmentId == null ? "" : Model.DepartmentId.ToString()));
            return View(Model);
        }


        [Route("Home/MaximoStatistics")]
        [HttpGet]
        public IActionResult MaximoStatistics()
        {
            string FROMData = Request.Cookies["maxstatfrom"];
            string TOData = Request.Cookies["maxstatto"];
            string DepartmentIdData = Request.Cookies["maxstatdid"];
            string StatusIdData = Request.Cookies["maxstatstatusid"];

            long Did = 0;
            long Sid = 0;
            DateTime From, To;
            if (!DateTime.TryParse(FROMData, out From))
            {
                From = DateTime.Now.Date.AddDays(-2);
            }
            if (!DateTime.TryParse(TOData, out To))
            {
                To = DateTime.Now;
            }
            FROMData = From.ToString("yyyy-MM-ddTHH:mm");
            TOData = To.ToString("yyyy-MM-ddTHH:mm");
            long.TryParse(DepartmentIdData, out Did);
            long.TryParse(StatusIdData, out Sid);

            ViewBag.Data = Helpers.DBHelper.GetMaximoStatistics(From:From, To:To, DeptId:(Did == 0 ? null : Did), StatusId:(Sid == 0 ? null : Sid), UserDept: ((Credentials)HttpContext.Items["User"]).DeptId, logger: _logger);
            Response.Cookies.Append("maxstatfrom", FROMData);
            Response.Cookies.Append("maxstatto", TOData);
            Response.Cookies.Append("maxstatdid", string.IsNullOrEmpty(DepartmentIdData) ? "" : DepartmentIdData);
            Response.Cookies.Append("maxstatstatusid", string.IsNullOrEmpty(StatusIdData) ? "" : StatusIdData);
            return View(new MaximoStatisticsFilter() 
            { 
                From = FROMData, 
                To = TOData,
                DepartmentId = (Did == 0 ? null : Did), 
                MaximoStatusId = (Sid == 0 ? null : Sid) });
        }
        [Route("Home/MaximoStatistics")]
        [HttpPost]
        public IActionResult MaximoStatistics(MaximoStatisticsFilter Model)
        {
            DateTime From = DateTime.Parse(Model.From);
            DateTime To = DateTime.Parse(Model.To);
            ViewBag.Data = Helpers.DBHelper.GetMaximoStatistics(From: From, To: To, DeptId: Model.DepartmentId, StatusId: Model.MaximoStatusId, UserDept: ((Credentials)HttpContext.Items["User"]).DeptId, logger: _logger);

            Response.Cookies.Append("maxstatfrom", Model.From);
            Response.Cookies.Append("maxstatto", Model.To);
            Response.Cookies.Append("maxstatdid", (Model.DepartmentId == null ? "" : Model.DepartmentId.ToString()));
            Response.Cookies.Append("maxstatstatusid", (Model.MaximoStatusId == null ? "" : Model.MaximoStatusId.ToString()));
            return View(Model);
        }

        [HttpGet]
        public IActionResult MaximoStatisticsEdit(int id)
        {
            var res = Helpers.DBHelper.GetMaximoStatistics(Id:id, logger:_logger).FirstOrDefault();
            if (res != null) return PartialView("MaximoStatisticsEdit", res);
            return View();
        }
        [HttpPost]
        public IActionResult MaximoStatisticsUpdate(MaximoDefect Model)
        {
            Helpers.DBHelper.MaximoStatisticsUpdate(Model, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Дефект обновлен.");
            return new JsonResult(0);
        }
        [HttpGet]
        public IActionResult MaximoStatisticsSend(int Id)
        {
            Helpers.DBHelper.MaximoStatisticsSend(Id, ((Credentials)HttpContext.Items["User"]).Id, _logger);
            AlertHelper.SaveMessage(HttpContext.Session, AlertType.Success, $"Дефект отправлен в очередь.");
            return RedirectToAction("MaximoStatistics", "Home");
        }



        [HttpGet]
        public IActionResult Start()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetFile(Guid guid)
        {
            AttachmentData data = DBHelper.GetAttachment(guid);
            if (data == null) return new JsonResult(new {result = "Файла не существует." });
            return File(data.Data, System.Net.Mime.MediaTypeNames.Application.Octet, data.FileName);
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
                return new JsonResult(Helpers.DBHelper.GetAssetParameterSets(UserDept: ((Credentials)HttpContext.Items["User"]).DeptId, _logger,DeptID:id));
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
                return new JsonResult(Helpers.DBHelper.GetAssetParameterSets(UserDept: ((Credentials)HttpContext.Items["User"]).DeptId, _logger, id));
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
        [HttpGet]
        public JsonResult GetAssetChilds(string ObjId)
        {
            int id = 0;
            if (int.TryParse(ObjId, out id))
                return new JsonResult(Helpers.DBHelper.GetAssetChildsFromAsset(id, _logger));
            else
                return new JsonResult(new List<Hierarchy>());
        }
        [HttpGet]
        public JsonResult GetControlsByAssetChilds(string ObjId)
        {
            int id = 0;
            if (int.TryParse(ObjId, out id))
                return new JsonResult(Helpers.DBHelper.GetControlsFromAssetChilds(id, _logger));
            else
                return new JsonResult(new List<Hierarchy>());
        }
        #endregion
    }
}
