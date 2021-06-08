using System;

namespace O2GEN.Models
{
    /// <summary>
    /// Контроли
    /// </summary>
    public class Control
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string ValueType { get; set; }
        public string DisplayValueType { get; set; }
        public Guid ObjectUID { get; set; }
    }
}
