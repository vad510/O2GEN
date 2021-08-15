using System;
using System.Collections.Generic;

namespace O2GEN.Models
{
    /// <summary>
    /// Отчёт по значениям контролей
    /// </summary>
    public class ControlValueReportData
    {
        /// <summary>
        /// Список обходов
        /// </summary>
        public Dictionary<long, DateTime> SCs { get; set; } = new Dictionary<long, DateTime>();
        /// <summary>
        /// Список строк
        /// </summary>
        public List<ControlValueReportRow> Rows { get; set; } = new List<ControlValueReportRow>();
    }
    public class ControlValueReportRow 
    { 
        public long AId { get; set; }
        public string AName { get; set; }
        public long ACId { get; set; }
        public string ACName { get; set; }
        public long APId { get; set; }
        public string APName { get; set; }
        /// <summary>
        /// Значение с датой
        /// </summary>
        public Dictionary<long, string> Data { get; set; } = new Dictionary<long, string>();
    }
}
