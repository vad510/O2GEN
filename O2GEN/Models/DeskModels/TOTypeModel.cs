using System.ComponentModel;

namespace O2GEN.Models.DeskModels
{
    public class TOTypeModel
    {
        [DisplayName("Название")]
        public string Name { get; set; }

        [DisplayName("Отображаемое название")]
        public string VisibleName { get; set; }

        [DisplayName("")]
        public string Edit { get; set; }
    }
}
