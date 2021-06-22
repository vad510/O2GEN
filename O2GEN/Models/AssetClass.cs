using System;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Классы объектов
    /// </summary>
    public class AssetClass
    {
        public int Id { get; set; } = -1;

        [DisplayName("Название")]
        public string DisplayName { get; set; }
        public Guid ObjectUID { get; set; }
    }
}
