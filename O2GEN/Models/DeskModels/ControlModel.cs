using System.ComponentModel;

namespace O2GEN.Models.DeskModels
{
    public class ControlModel
    {
        [DisplayName("Отображаемое название")]
        public string Name { get; set; }

        [DisplayName("Тип")]
        public string Type { get; set; }

        [DisplayName("")]
        public string Edit{ get; set; }
    }
}
