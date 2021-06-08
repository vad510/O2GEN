using System;

namespace O2GEN.Models
{
    /// <summary>
    /// Работники
    /// </summary>
    public class Engineer
    {
        public int Id { get; set; }
        public string PersonName { get; set; }
        public string DepartmentName { get; set; }
        public bool IsUser { get; set; }
        public Guid ObjectUID { get; set; }
    }
}
