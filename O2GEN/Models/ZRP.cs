using System;
using System.ComponentModel;

namespace O2GEN.Models
{

    public class ZRP
    {
        public int Id { get; set; } = -1;
        /// <summary>
        /// Дата начала
        /// </summary>
        [DisplayName("Дата начала (план)")]
        public DateTime StartTime { get; set; } = DateTime.Now.Date;
        /// <summary>
        /// Дата  окончания
        /// </summary>
        [DisplayName("Дата окончания (план)")]
        public DateTime EndTime { get; set; } = DateTime.Now.Date.AddDays(1);
        public int ObjId { get; set; }
        /// <summary>
        /// Тип оборудования
        /// </summary>
        [DisplayName("Тип оборудований")]
        public string ObjName { get; set; }

        [DisplayName("Тип обхода")]
        public int TypeId { get; set; }
        /// <summary>
        /// Тип Обхода
        /// </summary>
        [DisplayName("Тип обхода")]
        public string TypeName { get; set; }
        [DisplayName("Статус обхода")]
        public int StatusId { get; set; }
        [DisplayName("Статус обхода")]
        public string StatusName { get; set; }
        /// <summary>
        /// Обход
        /// </summary>
        [DisplayName("Обход")]
        public string RouteName { get; set; }
        /// <summary>
        /// Обходчик
        /// </summary>
        [DisplayName("Обходчик")]
        public string ResName { get; set; }
        [DisplayName("Обходчик")]
        public int? ResourceId { get; set; }

        [DisplayName("Подразделение")]
        public string Department { get; set; }

        [DisplayName("Подразделение")]
        public int? DepartmentID { get; set; }
    }
}
