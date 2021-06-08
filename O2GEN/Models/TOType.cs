using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2GEN.Models
{
    /// <summary>
    /// Типы ТО
    /// </summary>
    public class TOType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Guid ObjectUID { get; set; }
    }
}
