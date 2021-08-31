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
        public long AssetParameterTypeId { get; set; }

        [DisplayName("Название")]
        public string DisplayValueType { get; set; }

        [DisplayName("от")]
        public string ValueBottom1 { get; set; } = "-999999999999";

        [DisplayName("до")]
        [DoubleCompareAttribute("ValueBottom1", "AssetParameterTypeId", ErrorMessage = "Значение 'Норма от' должно быть меньше 'Норма до'")]
        public string ValueTop1 { get; set; } = "999999999999";

        [DisplayName("от")]
        [DoubleCompareAttribute("ValueTop1", "AssetParameterTypeId", ErrorMessage = "Значение 'Норма до' должно быть меньше 'Отклонение от'")]
        public string ValueBottom2 { get; set; } = "-999999999999";

        [DisplayName("до")]
        [DoubleCompareAttribute("ValueBottom2", "AssetParameterTypeId", ErrorMessage = "Значение 'Значение 'Отклонение от' должно быть меньше 'Отклонение до'")]
        public string ValueTop2 { get; set; } = "999999999999";

        [DisplayName("от")]
        [DoubleCompareAttribute("ValueTop2", "AssetParameterTypeId", ErrorMessage = "Значение 'Значение 'Отклонение до' должно быть меньше 'Сильное отклонение от'")]
        public string ValueBottom3 { get; set; } = "-999999999999";

        [DisplayName("до")]
        [DoubleCompareAttribute("ValueBottom3", "AssetParameterTypeId", ErrorMessage = "Значение 'Значение 'Сильное отклонение от' должно быть меньше 'Сильное отклонение до'")]
        public string ValueTop3 { get; set; } = "999999999999";



        public Guid ObjectUID { get; set; } = Guid.NewGuid();
    }
}
