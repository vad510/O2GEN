using System.ComponentModel;

namespace O2GEN.Models.DeskModels
{
    public class RouteModel
    {
        [DisplayName("Отображаемое название")]
        public string Name { get; set; }

        [DisplayName("Подразделение")]
        public string SubDivision{ get; set; }

        [DisplayName("")]
        public string Edit { get; set; }
    }
}
