using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace O2GEN
{
    public class TestClass
    {
        [DisplayName("Тип оборудований")]
        public string EquipmentType { get; set; }

        [DisplayName("Статус обхода")]
        public string Status { get; set; }

        [DisplayName("Тип обхода")]
        public string Type { get; set; }

        [DisplayName("Обход")]
        public string Bypass { get; set; }

        [DisplayName("Обходчик")]
        public string Bypasser { get; set; }

        [DisplayName("Дата начала (план)")]
        public DateTime Start { get; set; }

        [DisplayName("Дата окончания (план)")]
        public DateTime End { get; set; }
        [DisplayName("Дефект")]
        public string Defect { get; set; }

        [DisplayName("Редактирование")]
        public string Edit { get; set; }
    }
}
