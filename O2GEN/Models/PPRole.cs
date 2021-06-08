using System;

namespace O2GEN.Models
{
    /// <summary>
    /// Роли
    /// </summary>
    public class PPRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Guid ObjectUID { get; set; }
    }
}
