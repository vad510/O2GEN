using System;
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
        public Guid ObjectUID { get; set; }
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
        [DisplayName("Участок")]
        public int? DepartmentId { get; set; }
    }
}
