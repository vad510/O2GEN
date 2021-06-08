using System;

namespace O2GEN.Models
{
    /// <summary>
    /// Маршрут
    /// </summary>
    public class AssetParameterSet
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string DepartmentName { get; set; }
        public Guid ObjectUID { get; set; }
    }
}
