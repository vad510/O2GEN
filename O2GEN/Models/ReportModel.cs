using O2GEN.Models.HomeModels;

namespace O2GEN.Models
{
    /// <summary>
    /// HOME controller data
    /// </summary>
    public class ReportModel
    {
        public ZprTakenModels ZprTakenModels { get; set; } = new();

        public DefectReportListModel DefectReports { get; set; } = new();

        public ControlValueReportListModel ControlValueReportModels { get; set; } = new();


        public ZprTakenModel SelectedZprTakenModel { get; set; }

        public ControlValueReportModel SelectedControlValueReportModel { get; set; }

        public ReportOnViewedTechPosition SelectedReportOnViewedTechPosition { get; set; }

        public ReportOnViewedTechPosition SelectedReportOnViewedTechPosition2 { get; set; }

        public DefectReportModel SelectedDefectReportModel { get; set; }
    }
}
