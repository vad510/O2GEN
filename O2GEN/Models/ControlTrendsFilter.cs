namespace O2GEN.Models
{
    public class ControlTrendsFilter
    {
        public string From { get; set; }
        public string To { get; set; }
        public long? DepartmentId { get; set; }
        public long? AssetParameterSetId { get; set; }
        public long? AssetId { get; set; }
        public long? AssetChildId { get; set; }
        public long? AssetParameterId { get; set; }
    }
}
