using O2GEN.Helpers.ValidationAttributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace O2GEN.Models
{
    /// <summary>
    /// Контроли
    /// </summary>
    [DataContract]
    public class ControlStatistic
    {
        [DataMember(Name = "x")]
        public double? x { get; set; }
        [DataMember(Name = "y")]
        public double? y { get; set; }
    }
}
