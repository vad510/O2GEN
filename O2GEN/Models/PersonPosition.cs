using System;

namespace O2GEN.Models
{
    /// <summary>
    /// Должность
    /// </summary>
    public class PersonPosition
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public Guid ObjectUID { get; set; }
    }
}
