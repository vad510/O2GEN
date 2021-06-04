using System.Collections.Generic;
using System.ComponentModel;

namespace O2GEN.Models.EmployeeModels
{
    public class DepartmentHeadModel : Entity
    {
        [DisplayName("Название")]
        public string Name { get; set; }

        [DisplayName("Отображаемое значение")]
        public string DisplayName { get; set; }

        [DisplayName("Родительский участок")]
        public string ParentArea { get; set; }

        [DisplayName("Организация")]
        public string Organization { get; set; }

        [DisplayName("Широта")]
        public string Latitude { get; set; }

        [DisplayName("Долгота")]
        public string Longitude { get; set; }

        [DisplayName("Часовой пояс")]
        public string TimeZone { get; set; }

        [DisplayName("Короткий код")]
        public string ShortCode { get; set; }

        public List<DepartmentModel> DepartmentModels { get; set; } = new List<DepartmentModel>();


        [DisplayName("")]
        public string Edit { get; set; }

    }
}
