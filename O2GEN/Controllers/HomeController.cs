using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2GEN.Authorization;
using O2GEN.Helpers;
using O2GEN.Models;
using O2GEN.Models.HomeModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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
            string FROMData = Request.Cookies["zrpfrom"];
            string TOData = Request.Cookies["zrpto"];
            string DepartmentIdData = Request.Cookies["zrpdid"];
            string DisplayName = Request.Cookies["zrpn"];

            long FromL = 0;
            long ToL = 0;
            long Did = 0;
            if (string.IsNullOrEmpty(FROMData) || !long.TryParse(FROMData, out FromL)) 
            {
                FROMData = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TOData) || !long.TryParse(TOData, out ToL))
            {
                TOData = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(1).Ticks).ToString();
            }
            if(!long.TryParse(DepartmentIdData, out Did)) Did = ((Credentials)HttpContext.Items["User"]).DeptId;
            DateTime From = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(FROMData)));
            DateTime To = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(TOData)));

            //ViewBag.ZRPCreated = Helpers.DBHelper.GetZRP(From, To, _logger, (int)Helpers.ZRPStatus.Created, (Did == 0 ? null : Did));
            ViewBag.ZRPInWork = Helpers.DBHelper.GetZRP(From, To, _logger, new int[] { (int)Helpers.ZRPStatus.Started, (int)Helpers.ZRPStatus.Created }, DisplayName, (Did == 0 ? null : Did), ((Credentials)HttpContext.Items["User"]).DeptId);
            ViewBag.ZRPDone = Helpers.DBHelper.GetZRP(From, To, _logger, new int[] { (int)Helpers.ZRPStatus.Ended, (int)Helpers.ZRPStatus.Stoped }, DisplayName, (Did == 0 ? null : Did), ((Credentials)HttpContext.Items["User"]).DeptId);
            Response.Cookies.Append("zrpfrom", FROMData);
            Response.Cookies.Append("zrpto", TOData);
            Response.Cookies.Append("zrpdid", string.IsNullOrEmpty(DepartmentIdData)?"" : DepartmentIdData);
            Response.Cookies.Append("zrpn", (string.IsNullOrEmpty(DisplayName) ? "" : DisplayName));
            AlertHelper.DisplayMessage(HttpContext.Session, ViewBag);
            return View(new ZRPFilter() { From = Helpers.DateTimeHelper.TicksFromNETToJS(From.Ticks).ToString(), To = Helpers.DateTimeHelper.TicksFromNETToJS(To.Ticks).ToString(), DepartmentId = (Did == 0 ? null : Did), DisplayName = DisplayName });
        }
        [Route("Home/Index")]
        [HttpPost]
        public IActionResult Index(ZRPFilter Model)
        {
            DateTime From = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.From)));
            DateTime To = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.To)));
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
                FROMData = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TOData) || !long.TryParse(TOData, out TOParsed))
            {
                TOData = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            if (!int.TryParse(DIdData, out DIdParsed)) DIdParsed = (int)((Credentials)HttpContext.Items["User"]).DeptId;
            int.TryParse(APSIdData, out APSIdParsed);
            int.TryParse(APIdData, out APIdParsed);
            DateTime From = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(FROMData)));
            DateTime To = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(TOData)));

            ViewBag.Data = null;
            Response.Cookies.Append("RepCVFrom", FROMData);
            Response.Cookies.Append("RepCVTo", TOData);
            Response.Cookies.Append("RepCVDId", string.IsNullOrEmpty(DIdData) ? "" : DIdData);
            Response.Cookies.Append("RepCVAPSId", string.IsNullOrEmpty(APSIdData) ? "" : APSIdData);
            Response.Cookies.Append("RepCVAPId", string.IsNullOrEmpty(APIdData) ? "" : APIdData);
            return View(new ControlValueReportFilter()
            {
                From = Helpers.DateTimeHelper.TicksFromNETToJS(From.Ticks).ToString(),
                To = Helpers.DateTimeHelper.TicksFromNETToJS(To.Ticks).ToString(),
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
            DateTime From = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.From)));
            DateTime To = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.To)));
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
                FROM1Data = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TO1Data) || !long.TryParse(TO1Data, out TO1Parsed))
            {
                TO1Data = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            if (string.IsNullOrEmpty(FROM2Data) || !long.TryParse(FROM2Data, out FROM2Parsed))
            {
                FROM2Data = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TO2Data) || !long.TryParse(TO2Data, out TO2Parsed))
            {
                TO2Data = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            DateTime From1 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(FROM1Data)));
            DateTime To1 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(TO1Data)));
            DateTime From2 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(FROM2Data)));
            DateTime To2 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(TO2Data)));

            ViewBag.Data = null;
            Response.Cookies.Append("RepVAFrom1", FROM1Data);
            Response.Cookies.Append("RepVATo1", TO1Data);
            Response.Cookies.Append("RepVAFrom2", FROM2Data);
            Response.Cookies.Append("RepVATo2", TO2Data);
            return View(new AssetsReportFilter()
            {
                From1 = Helpers.DateTimeHelper.TicksFromNETToJS(From1.Ticks).ToString(),
                To1 = Helpers.DateTimeHelper.TicksFromNETToJS(To1.Ticks).ToString(),
                From2 = Helpers.DateTimeHelper.TicksFromNETToJS(From2.Ticks).ToString(),
                To2 = Helpers.DateTimeHelper.TicksFromNETToJS(To2.Ticks).ToString()
            });
        }
        [Route("Home/ReportsOnViewedTechPositions")]
        [HttpPost]
        public IActionResult ReportsOnViewedTechPositions(AssetsReportFilter Model)
        {
            DateTime From1 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.From1)));
            DateTime To1 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.To1)));
            DateTime From2 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.From2)));
            DateTime To2 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.To2)));

            ViewBag.Data = new AssetsReportMerge(Helpers.DBHelper.GetDepartments(((Credentials)HttpContext.Items["User"]).DeptId, logger: _logger), Helpers.DBHelper.GetVA(From1, To1, _logger), Helpers.DBHelper.GetVA(From2, To2, _logger));

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
                FROM1Data = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TO1Data) || !long.TryParse(TO1Data, out TO1Parsed))
            {
                TO1Data = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            if (string.IsNullOrEmpty(FROM2Data) || !long.TryParse(FROM2Data, out FROM2Parsed))
            {
                FROM2Data = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TO2Data) || !long.TryParse(TO2Data, out TO2Parsed))
            {
                TO2Data = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            DateTime From1 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(FROM1Data)));
            DateTime To1 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(TO1Data)));
            DateTime From2 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(FROM2Data)));
            DateTime To2 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(TO2Data)));

            ViewBag.Data = null;
            Response.Cookies.Append("RepNAFrom1", FROM1Data);
            Response.Cookies.Append("RepNATo1", TO1Data);
            Response.Cookies.Append("RepNAFrom2", FROM2Data);
            Response.Cookies.Append("RepNATo2", TO2Data);
            return View(new AssetsReportFilter()
            {
                From1 = Helpers.DateTimeHelper.TicksFromNETToJS(From1.Ticks).ToString(),
                To1 = Helpers.DateTimeHelper.TicksFromNETToJS(To1.Ticks).ToString(),
                From2 = Helpers.DateTimeHelper.TicksFromNETToJS(From2.Ticks).ToString(),
                To2 = Helpers.DateTimeHelper.TicksFromNETToJS(To2.Ticks).ToString()
            });
        }
        [Route("Home/NumberOfTechPosReports")]
        [HttpPost]
        public IActionResult NumberOfTechPosReports(AssetsReportFilter Model)
        {
            DateTime From1 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.From1)));
            DateTime To1 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.To1)));
            DateTime From2 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.From2)));
            DateTime To2 = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.To2)));

            ViewBag.Data = new AssetsReportMerge(Helpers.DBHelper.GetDepartments(((Credentials)HttpContext.Items["User"]).DeptId, logger: _logger), Helpers.DBHelper.GetNA(From1, To1, _logger), Helpers.DBHelper.GetNA(From2, To2, _logger));

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

            long FromL = 0,
            ToL = 0,
            DId = 0,
            APSId = 0,
            AId = 0,
            ACId = 0,
            APId = 0;
            if (string.IsNullOrEmpty(FROMData) || !long.TryParse(FROMData, out FromL))
            {
                FROMData = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TOData) || !long.TryParse(TOData, out ToL))
            {
                TOData = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Ticks).ToString();
            }
            DateTime From = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(FROMData)));
            DateTime To = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(TOData)));
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
                    From = Helpers.DateTimeHelper.TicksFromNETToJS(From.Ticks).ToString(), 
                    To = Helpers.DateTimeHelper.TicksFromNETToJS(To.Ticks).ToString(), 
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
            DateTime From = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.From)));
            DateTime To = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.To)));
            if(Model.DepartmentId is null ||
                Model.AssetParameterSetId is null ||
                Model.AssetId is null ||
                Model.AssetChildId is null ||
                Model.AssetParameterId is null )
            {
                ViewBag.Message = "Данные для указанного фильтра не найдены.";
            }
            else
            {
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

            long FromL = 0;
            long ToL = 0;
            long Did = 0;
            if (string.IsNullOrEmpty(FROMData) || !long.TryParse(FROMData, out FromL))
            {
                FROMData = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TOData) || !long.TryParse(TOData, out ToL))
            {
                TOData = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(1).Ticks).ToString();
            }
            if (!long.TryParse(DepartmentIdData, out Did)) Did = ((Credentials)HttpContext.Items["User"]).DeptId;
            DateTime From = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(FROMData)));
            DateTime To = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(TOData)));

            ViewBag.Data = Helpers.DBHelper.GetStatistics(From, To, (Did == 0 ? null : Did), _logger);
            Response.Cookies.Append("statfrom", FROMData);
            Response.Cookies.Append("statto", TOData);
            Response.Cookies.Append("statdid", string.IsNullOrEmpty(DepartmentIdData) ? "" : DepartmentIdData);
            return View(new StatisticsFilter() { From = Helpers.DateTimeHelper.TicksFromNETToJS(From.Ticks).ToString(), To = Helpers.DateTimeHelper.TicksFromNETToJS(To.Ticks).ToString(), DepartmentId = (Did == 0 ? null : Did) });
        }
        [Route("Home/Statistics")]
        [HttpPost]
        public IActionResult Statistics(StatisticsFilter Model)
        {
            DateTime From = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.From)));
            DateTime To = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.To)));
            ViewBag.Data = Helpers.DBHelper.GetStatistics(From, To, Model.DepartmentId, _logger);

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

            long FromL = 0;
            long ToL = 0;
            long Did = 0;
            long Sid = 0;
            if (string.IsNullOrEmpty(FROMData) || !long.TryParse(FROMData, out FromL))
            {
                FROMData = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(-2).Ticks).ToString();
            }
            if (string.IsNullOrEmpty(TOData) || !long.TryParse(TOData, out ToL))
            {
                TOData = Helpers.DateTimeHelper.TicksFromNETToJS(DateTime.Now.Date.AddDays(1).Ticks).ToString();
            }
            long.TryParse(DepartmentIdData, out Did);
            long.TryParse(StatusIdData, out Sid);
            DateTime From = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(FROMData)));
            DateTime To = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(TOData)));

            ViewBag.Data = Helpers.DBHelper.GetMaximoStatistics(From:From, To:To, DeptId:(Did == 0 ? null : Did), StatusId:(Sid == 0 ? null : Sid), UserDept: ((Credentials)HttpContext.Items["User"]).DeptId, logger: _logger);
            Response.Cookies.Append("maxstatfrom", FROMData);
            Response.Cookies.Append("maxstatto", TOData);
            Response.Cookies.Append("maxstatdid", string.IsNullOrEmpty(DepartmentIdData) ? "" : DepartmentIdData);
            Response.Cookies.Append("maxstatstatusid", string.IsNullOrEmpty(StatusIdData) ? "" : StatusIdData);
            return View(new MaximoStatisticsFilter() { From = Helpers.DateTimeHelper.TicksFromNETToJS(From.Ticks).ToString(), To = Helpers.DateTimeHelper.TicksFromNETToJS(To.Ticks).ToString(), DepartmentId = (Did == 0 ? null : Did), MaximoStatusId = (Sid == 0 ? null : Sid) });
        }
        [Route("Home/MaximoStatistics")]
        [HttpPost]
        public IActionResult MaximoStatistics(MaximoStatisticsFilter Model)
        {
            DateTime From = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.From)));
            DateTime To = new DateTime().AddTicks(Helpers.DateTimeHelper.TicksFromJSToNET(long.Parse(Model.To)));
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
