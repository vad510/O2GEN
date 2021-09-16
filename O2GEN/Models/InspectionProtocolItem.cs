using System;
using System.ComponentModel;

namespace O2GEN.Models
{

    public class InspectionProtocolItem
    {
        [DisplayName("Контроль")]
        public string Name { get; set; }
        [DisplayName("Дата проведения осмотра")]
        public DateTime? Date { get; set; }
        [DisplayName("Значение")]
        public string Value { get; set; }
        [DisplayName("Комментарий")]
        public string Comment { get; set; }
    }
}
