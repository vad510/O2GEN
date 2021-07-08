using System;
using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Контроли
    /// </summary>
    public class Control
    {
        public int Id { get; set; } = -1;

        [DisplayName("Название")]
        public string DisplayName { get; set; }

        [DisplayName("Тип контроля")]
        public int ValueType { get; set; }

        [DisplayName("Название")]
        public string DisplayValueType { get; set; }

        [DisplayName("от")]
        public string ValueBottom1 { get; set; } = "-999999999999";
        [DisplayName("до")]
        public string ValueTop1 { get; set; } = "999999999999";
        [DisplayName("от")]
        public string ValueBottom2 { get; set; } = "-999999999999";
        [DisplayName("до")]
        public string ValueTop2 { get; set; } = "999999999999";
        [DisplayName("от")]
        public string ValueBottom3 { get; set; } = "-999999999999";
        [DisplayName("до")]
        public string ValueTop3 { get; set; } = "999999999999";



        public Guid ObjectUID { get; set; } = Guid.NewGuid();
    }
}
