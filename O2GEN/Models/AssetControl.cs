using O2GEN.Helpers.ValidationAttributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace O2GEN.Models
{
    /// <summary>
    /// Контроли в узле
    /// </summary>
    public class AssetControl
    {
        public long AssetParameterId { get; set; } = -1;
        /// <summary>
        /// Id пары AssetParameterPairs
        /// </summary>
        public long? PairId { get; set; } = -1;
        public string DisplayName { get; set; }
        public bool IsPair { get; set; } = false;
        /// <summary>
        /// Номер группы
        /// </summary>
        public string GroupNum { get; set; } = "-";
        public Guid? ObjectUID { get; set; }
    }
}
