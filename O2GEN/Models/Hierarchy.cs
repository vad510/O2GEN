using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Объект иерархии
    /// </summary>
    public class Hierarchy
    {
        public int Id { get; set; } = -1;
        public string DisplayName { get; set; }
        public List<Hierarchy> Childs { get; set; } = new List<Hierarchy>();
    }
}
