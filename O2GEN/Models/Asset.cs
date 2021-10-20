using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Объекты
    /// </summary>
    public class Asset
    {
        public int Id { get; set; } = -1;
        [DisplayName("Наименование")]
        public string DisplayName { get; set; }
        [DisplayName("Тех. позиция")]
        public string Maximo { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<Asset> Childs { get; set; }
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
        [DisplayName("Родительский объект")]
        public int? ParentId { get; set; }
        [DisplayName("Класс объектов")]
        public int? AssetClassId { get; set; } = null;
        [DisplayName("Тип объекта")]
        public int? AssetSortId { get; set; }
        [DisplayName("Подразделение")]
        public int? DepartmentId { get; set; }
        [DisplayName("Статус оборудования")]
        public long? AssetStateId { get; set; }
        public List<int> Parameters { get; set; } = new List<int>();

        [DisplayName("Создан")]
        public DateTime CreateStamp { get; set; }
        [DisplayName("Редактирован")]
        public DateTime ModifyStamp { get; set; }

    }
}
