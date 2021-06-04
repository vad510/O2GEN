using System;
using System.ComponentModel;

namespace O2GEN.Models.HomeModels
{
    public class ControlValueReportModel
    {
        [DisplayName("Тех. позиция")]
        public string TechPosition { get; set; }

        [DisplayName("Узел")]
        public string Node { get; set; }

        [DisplayName("Контроль")]
        public string Control { get; set; }

        public DateTime TimeCreated { get; set; }

        public string Attachment { get; set; }
    }
}
