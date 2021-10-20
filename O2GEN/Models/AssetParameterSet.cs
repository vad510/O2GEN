using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "Укажите подразделение")]
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
        public List<Hierarchy> Childs { get; set; } = new List<Hierarchy>();

        [DisplayName("Создан")]
        public DateTime CreateStamp { get; set; }
        [DisplayName("Редактирован")]
        public DateTime ModifyStamp { get; set; }
    }
}
