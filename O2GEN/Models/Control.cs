using O2GEN.Helpers.ValidationAttributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace O2GEN.Models
{
    /// <summary>
    /// Контроли
    /// </summary>
    public class Control
    {
        public int Id { get; set; } = -1;

        [DisplayName("Название")]
        [Required(ErrorMessage = "Укажите название")]
        [MinLength(1)]
        public string DisplayName { get; set; }

        [DisplayName("Тип контроля")]
        public int ValueType { get; set; }

        [DisplayName("Название")]
        public string DisplayValueType { get; set; }

        [DisplayName("от")]
        [DoubleCompareAttribute("ValueTop1", ErrorMessage = "Значение 'Норма от' должно быть меньше 'Норма до'")]
        public string ValueBottom1 { get; set; } = "-999999999999";

        [DisplayName("до")]
        public string ValueTop1 { get; set; } = "999999999999";

        [DisplayName("от")]
        [DoubleCompareAttribute("ValueTop2", ErrorMessage = "Значение 'Норма от' должно быть меньше 'Норма до'")]
        public string ValueBottom2 { get; set; } = "-999999999999";

        [DisplayName("до")]
        public string ValueTop2 { get; set; } = "999999999999";

        [DisplayName("от")]
        [DoubleCompareAttribute("ValueTop3", ErrorMessage = "Значение 'Норма от' должно быть меньше 'Норма до'")]
        public string ValueBottom3 { get; set; } = "-999999999999";

        [DisplayName("до")]
        public string ValueTop3 { get; set; } = "999999999999";



        public Guid ObjectUID { get; set; } = Guid.NewGuid();
    }
}
