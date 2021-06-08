using System;
using System.Collections.Generic;

namespace O2GEN.Models
{
    /// <summary>
    /// Участки
    /// </summary>
    public class Department
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public List<Department> Childs { get; set; }
        public int? ParentId { get; set; }
        public Guid ObjectUID { get; set; }
    }
}
