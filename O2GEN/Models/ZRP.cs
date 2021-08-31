using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using O2GEN.Helpers.ValidationAttributes;

namespace O2GEN.Models
{

    public class ZRP
    {
        public int Id { get; set; } = -1;
        public Guid? ObjectUID { get; set; } = Guid.NewGuid();
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
        [Required(ErrorMessage = "Укажите тип обхода")]
        public int? SCTypeId { get; set; }
        /// <summary>
        /// Тип Обхода
        /// </summary>
        [DisplayName("Тип обхода")]
        public string SCTypeName { get; set; }
        [DisplayName("Статус обхода")]
        public int? SCStatusId { get; set; }
        [DisplayName("Статус обхода")]
        public string SCStatusName { get; set; }
        /// <summary>
        /// Обход
        /// </summary>
        [DisplayName("Обход")]
        [Required(ErrorMessage = "Укажите название")]
        public string RouteName { get; set; }
        /// <summary>
        /// Обходчик
        /// </summary>
        [DisplayName("Обходчик")]
        public string ResName { get; set; }
        //[DisplayName("Обходчик")]
        //[Required(ErrorMessage = "Укажите бригаду")]
        [DisplayName("Должность")]
        [Required(ErrorMessage = "Укажите должность")]
        public int? ResourceId { get; set; }

        [DisplayName("Подразделение")]
        public string Department { get; set; }

        [DisplayName("Подразделение")]
        [Required(ErrorMessage = "Укажите подразделение")]
        public int? DepartmentID { get; set; }
        [Obsolete]
        [DisplayName("Дни:")]
        public int PeriodDays { get; set; }
        [Obsolete]
        [DisplayName("Часы:")]
        public int PeriodHours { get; set; }
        [Obsolete]
        [DisplayName("Минуты:")]
        public int PeriodMinutes { get; set; }
        /// <summary>
        /// Не думаю что имеет смысл в передаче, у нас будет список AssetParameter
        /// </summary>
        [DisplayName("Маршрут")]
        [Required(ErrorMessage = "Выберите подразделение")]
        public int? AssetParameterSetId { get; set; }
        /// <summary>
        /// Инфа по техпозициям уже существующего обхода.
        /// </summary>
        public List<InspectionProtocol> InsProt { get; set; } = new List<InspectionProtocol>();
        /// <summary>
        /// Инфа по техпозициям для создаваемой пачки обходов.
        /// </summary>
        public List<Hierarchy> NewTechPoz { get; set; } = new List<Hierarchy>();
    }
}
