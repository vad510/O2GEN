using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Бригады
    /// </summary>
    public class Resource
    {
        public int Id { get; set; } = -1;
        [DisplayName("ФИО (Название)")]
        public string DisplayName { get; set; }
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
        [DisplayName("Широта")]
        public double? Latitude { get; set; }
        [DisplayName("Долгота")]
        public double? Longitude { get; set; }
        [DisplayName("Адрес")]
        public string Address { get; set; }
        [DisplayName("Тип бригады")]
        public int? ResourceTypeId { get; set; }
        [DisplayName("Состояние ресурса")]
        public int? ResourceStateId { get; set; }
        [DisplayName("Подразделение")]
        public int? DepartmentId { get; set; }
        /// <summary>
        /// Список Emngineer.Id для текущей сущности
        /// </summary>
        public List<int> Parameters { get; set; } = new List<int>();
        [DisplayName("Создан")]
        public DateTime CreateStamp { get; set; }
        [DisplayName("Редактирован")]
        public DateTime ModifyStamp { get; set; }
    }
}
