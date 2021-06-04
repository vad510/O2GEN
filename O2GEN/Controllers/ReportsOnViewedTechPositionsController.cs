//using Microsoft.AspNetCore.Mvc;
//using O2GEN.Models.ReportsOnViewedTechModels;
//using System;

//namespace O2GEN.Controllers
//{
//    public class ReportsOnViewedTechPositionsController : Controller
//    {
//        private ReportOnViewedTechPositionList _reportOnViewedTechPositionList;

//        public ReportsOnViewedTechPositionsController()
//        {
//            _reportOnViewedTechPositionList = new ReportOnViewedTechPositionList();
//            _reportOnViewedTechPositionList.ReportOnViewedTechPositions.Add(new ReportOnViewedTechPosition
//            {
//                StationName = "1",
//                FirstInterval = DateTime.Now,
//                SecondInterval = DateTime.Now,
//                Changes = 20
//            });
//        }

//        public IActionResult Index()
//        {

//            return View(_reportOnViewedTechPositionList);
//        }
//    }
//}
