using System.ComponentModel;

namespace O2GEN.Models.EmployeeModels
{
    public class EmployeeModel
    {
        [DisplayName("Отображаемое название")]
        public string VisibleName { get; set; }

        [DisplayName("Подразделение")]
        public string Department { get; set; }

        [DisplayName("Пользователь")]
        public string UserType { get; set; }

        [DisplayName("")]
        public string Edit { get; set; }
    }
}
