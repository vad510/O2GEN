using System.ComponentModel;

namespace O2GEN.Models.DeskModels
{
    public class EmployeeCategroyModel
    {
        [DisplayName("Отображаемое название")]
        public string Name { get; set; }
        
        [DisplayName("")]
        public string Edit { get; set; }
    }
}
