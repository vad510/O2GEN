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
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
    }
}
