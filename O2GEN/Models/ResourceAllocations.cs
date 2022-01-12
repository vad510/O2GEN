using System;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Работники в бригаде
    /// </summary>
    public class ResourceAllocations
    {
        public long Id { get; set; } = -1;
        public long ResourceID { get; set; }
        public long EngineerID { get; set; }
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
        public string EngineerName { get; set; }
    }
}
