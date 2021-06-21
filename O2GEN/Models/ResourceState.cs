using System;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Объекты
    /// </summary>
    public class ResourceState
    {
        public int Id { get; set; } = -1;
        [DisplayName("Наименование")]
        public string DisplayName { get; set; }
        public Guid ObjectUID { get; set; }
    }
}
