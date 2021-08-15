namespace O2GEN.Models
{
    /// <summary>
    /// Фильтр для Отчета по значениям контролей
    /// </summary>
    public class ControlValueReportFilter
    {
        public string From { get; set; }
        public string To { get; set; }
        /// <summary>
        /// Подразделение
        /// </summary>
        public long? DepartmentId { get; set; }
        /// <summary>
        /// Мусорная информация
        /// </summary>
        public long? AssetParameterSetId { get; set; }
        /// <summary>
        /// Тех. позиция
        /// </summary>
        public long? AssetId { get; set; }
    }
}
