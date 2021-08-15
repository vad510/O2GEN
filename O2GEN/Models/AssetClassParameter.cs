using System;

namespace O2GEN.Models
{
    /// <summary>
    /// Контроли в классах объектов
    /// </summary>
    public class AssetClassParameter
    {
        public int Id { get; set; } = -1;
        public int AssetClassId { get; set; }
        public int AssetParameterId { get; set; }
        public string AssetParameterName { get; set; }
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
    }
}
