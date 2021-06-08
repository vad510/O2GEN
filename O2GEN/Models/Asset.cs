using System;
using System.Collections.Generic;

namespace O2GEN.Models
{
    /// <summary>
    /// Объекты
    /// </summary>
    public class Asset
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Maximo { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<Asset> Childs { get; set; }
        public Guid ObjectUID { get; set; }
        public int? ParentId { get; set; } 
    }
}
