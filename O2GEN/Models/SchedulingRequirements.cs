using System;
using System.ComponentModel;

namespace O2GEN.Models
{

    public class SchedulingRequirements
    {
        public int Id { get; set; } = -1;
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
        public int? DepartmentID { get; set; }
        public int? Resource { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
