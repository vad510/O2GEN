using System;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Работники в бригаде
    /// </summary>
    public class ResourceAllocations
    {
        public int Id { get; set; } = -1;
        public int ResourceID { get; set; }
        public int EngineerID { get; set; }
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
        public string EngineerName { get; set; }
    }
}
