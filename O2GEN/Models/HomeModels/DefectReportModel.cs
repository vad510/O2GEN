using System;
using System.ComponentModel;

namespace O2GEN.Models.HomeModels
{
    public class DefectReportModel
    {
        [DisplayName("Тех. позиция")]
        public string TechPos { get; set; }

        [DisplayName("Узел")]
        public string Node { get; set; }

        [DisplayName("Статус дефекта")]
        public string Status { get; set; }

        [DisplayName("Тип дефекта")]
        public string DefectType { get; set; }

        [DisplayName("Контроль")]
        public string Control { get; set; }

        [DisplayName("Значение")]
        public double Value { get; set; }

        [DisplayName("Дефект обнаружил")]
        public string DefectFoundedBy { get; set; }
        
        [DisplayName("Дефект обработал")]
        public string DefectProceededBy { get; set; }

        [DisplayName("Время обнаружения")]
        public DateTime DateDetected { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; }
    }
}
