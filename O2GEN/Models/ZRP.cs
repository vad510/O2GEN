using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace O2GEN.Models
{

    public class ZRP
    {
        public int Id { get; set; }
        /// <summary>
        /// Дата начала
        /// </summary>
        [DisplayName("Дата начала (план)")]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Дата  окончания
        /// </summary>
        [DisplayName("Дата окончания (план)")]
        public DateTime EndTime { get; set; }
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

        [DisplayName("Подразделение")]
        public string Department { get; set; }
    }
}
