using System;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Маршрут
    /// </summary>
    public class AssetParameterSet
    {
        public int Id { get; set; } = -1;

        [DisplayName("Название")]
        public string DisplayName { get; set; }
        [DisplayName("Подразделение")]
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
    }
}
