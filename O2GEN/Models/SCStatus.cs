using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Объекты
    /// </summary>
    public class SCStatus
    {
        public int Id { get; set; } = -1;
        [DisplayName("Наименование")]
        public string DisplayName { get; set; }
        [DisplayName("Индекс Максимо")]
        public string Maximo { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<Asset> Childs { get; set; }
        public Guid ObjectUID { get; set; }
        [DisplayName("Родительский объект")]
        public int? ParentId { get; set; }
        [DisplayName("Тип объекта")]
        public int? TypeID { get; set; }
        [DisplayName("Подразделение")]
        public int? DepartmentId { get; set; }


    }
}
