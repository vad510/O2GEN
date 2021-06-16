using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Участки
    /// </summary>
    public class Department
    {
        public int Id { get; set; }
        [DisplayName("Название")]
        public string Name { get; set; }
        [DisplayName("Отображаемое название")]
        public string DisplayName { get; set; }
        public List<Department> Childs { get; set; }
        public int? ParentId { get; set; }
        [DisplayName("Родительский участок")]
        public string ParentName { get; set; }
        public Guid ObjectUID { get; set; }
        [DisplayName("Широта")]
        public double Latitude { get; set; }
        [DisplayName("Долгота")]
        public double Longitude { get; set; }
        [DisplayName("Часовой пояс")]
        public string TimeZone { get; set; }
        [DisplayName("Организация")]
        public string Organization { get; set; }
        [DisplayName("Короткий код")]
        public string ShortCode { get; set; }
    }
}
