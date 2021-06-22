using System;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Должность
    /// </summary>
    public class PersonPosition
    {
        public int Id { get; set; } = -1;
        [DisplayName("Название")]
        public string Name { get; set; }
        [DisplayName("Отображаемое название")]
        public string DisplayName { get; set; }
        [DisplayName("Идентификатор объекта")]
        public Guid ObjectUID { get; set; }
    }
}
