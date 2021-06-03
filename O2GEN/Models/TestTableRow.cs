using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2GEN.Models
{
    /// <summary>
    /// Тестовая фигня  под запрос, потом  переименовать
    /// </summary>
    public class TestTableRow
    {
        public int Id { get; set; }
        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Дата  окончания
        /// </summary>
        public DateTime EndTime { get; set; }
        public int ObjId { get; set; }
        /// <summary>
        /// Тип оборудования
        /// </summary>
        public string ObjName { get; set; }
        public int TypeId { get; set; }
        /// <summary>
        /// Тип Обхода
        /// </summary>
        public string TypeName { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// Обход
        /// </summary>
        public string RouteName { get; set; }
        /// <summary>
        /// Обходчик
        /// </summary>
        public string ResName { get; set; }
    }
}
