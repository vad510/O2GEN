using System.Collections.Generic;
using System.ComponentModel;

namespace O2GEN.Models.DeskModels
{
    public class ObjectHeadModels
    {
        [DisplayName("Отображаемое название")]
        public string ObjectName { get; set; }

        [DisplayName("Индекс Максимо")]
        public string IndexMaximo { get; set; }

        [DisplayName("Статус объекта")]
        public string Status { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; }

        [DisplayName("")]
        public string Edit { get; set; }

        public List<ObjectModel> ObjectModels { get; set; } = new();
    }
}
