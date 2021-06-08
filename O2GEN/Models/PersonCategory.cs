using System;

namespace O2GEN.Models
{
    /// <summary>
    /// Категория персонала
    /// </summary>
    public class PersonCategory
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public Guid ObjectUID { get; set; }
    }
}
