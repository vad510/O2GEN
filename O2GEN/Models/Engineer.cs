using System;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Работники
    /// </summary>
    public class Engineer
    {
        public int Id { get; set; } = -1;
        [DisplayName("Фамилия")]
        public string Surname { get; set; }
        [DisplayName("Имя")]
        public string GivenName { get; set; }
        [DisplayName("Отчество")]
        public string MiddleName { get; set; }
        public string DisplayName { get; set; }
        public string AppointName { get; set; }
        public string PersonName { get; set; }
        public string DepartmentName { get; set; }
        public bool IsUser { get; set; }
        public Guid ObjectUID { get; set; }
        [DisplayName("Подразделение")]
        public int? DepartmentId { get; set; }
        [DisplayName("График")]
        public int? CalendarId { get; set; }
        public int? PersonId { get; set; }
        [DisplayName("Должность")]
        public int? PersonPositionId { get; set; }
    }
}
