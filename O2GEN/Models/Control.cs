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
        [Required(ErrorMessage = "Укажите тип контроля")]
        public long? AssetParameterTypeId { get; set; }

        [DisplayName("Название")]
        public string DisplayValueType { get; set; }

        [DisplayName("от")]
        public string ValueBottom1 { get; set; } = "-100";

        [DisplayName("до")]
        [DoubleCompareAttribute("ValueBottom1", "AssetParameterTypeId", ErrorMessage = "Значение 'Норма от' должно быть меньше 'Норма до'")]
        public string ValueTop1 { get; set; } = "100";

        [DisplayName("от")]
        public string ValueBottom2 { get; set; } = "-200";

        [DisplayName("до")]
        [DoubleCompareAttribute("ValueBottom2", "AssetParameterTypeId", ErrorMessage = "Значение 'Значение 'Отклонение от' должно быть меньше 'Отклонение до'")]
        public string ValueTop2 { get; set; } = "200";

        [DisplayName("от")]
        public string ValueBottom3 { get; set; } = "-300";

        [DisplayName("до")]
        [DoubleCompareAttribute("ValueBottom3", "AssetParameterTypeId", ErrorMessage = "Значение 'Значение 'Сильное отклонение от' должно быть меньше 'Сильное отклонение до'")]
        public string ValueTop3 { get; set; } = "300";

        /// <summary>
        /// Подразделение, указывать не обязательно
        /// </summary>
        [DisplayName("Подразделение")]
        public long? DepartmentId { get; set; }

        [DisplayName("Коментарий")]
        public string Description { get; set; } = "";

        public Guid ObjectUID { get; set; } = Guid.NewGuid();
    }
}
