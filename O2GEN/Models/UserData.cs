
using System.ComponentModel.DataAnnotations;
 
namespace O2GEN.Models
{
    public class UserData
    {
        /// <summary>
        /// Engineers.Id
        /// </summary>
        public long Id { get; set; }
        public long DeptId { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string GivenName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }
        /// <summary>
        /// Подразделение
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// Должность
        /// </summary>
        public string AppointName { get; set; }
        public string JWToken { get; set; }
    }
}