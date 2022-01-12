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
        public long Id { get; set; } = -1;
        [DisplayName("Название")]
        public string DisplayName { get; set; }
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
        [DisplayName("Подразделение")]
        public long? DepartmentId { get; set; }
        public List<Selectable<Engineer>> Engineers { get; set; } = new List<Selectable<Engineer>>();
        [DisplayName("Создан")]
        public DateTime CreateStamp { get; set; }
        [DisplayName("Редактирован")]
        public DateTime ModifyStamp { get; set; }
    }
}
