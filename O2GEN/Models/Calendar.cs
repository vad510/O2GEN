using System;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Должность
    /// </summary>
    public class Calendar
    {
        public int Id { get; set; } = -1;
        [DisplayName("Название")]
        public string Title { get; set; }
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
    }
}
