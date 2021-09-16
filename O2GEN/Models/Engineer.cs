using O2GEN.Helpers.ValidationAttributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace O2GEN.Models
{
    /// <summary>
    /// Работники
    /// </summary>
    public class Engineer
    {
        public int Id { get; set; } = -1;
        [DisplayName("Фамилия")]
        public string Surname { get; set; }
        [DisplayName("Имя")]
        public string GivenName { get; set; }
        [DisplayName("Отчество")]
        public string MiddleName { get; set; }
        public string DisplayName { get; set; }
        public string AppointName { get; set; }
        public string PersonName { get; set; }
        public string DepartmentName { get; set; }
        public bool IsUser { get; set; } = false;
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
        [DisplayName("Подразделение")]
        [Required(ErrorMessage = "Укажите подразделение")]
        public int? DepartmentId { get; set; }
        [DisplayName("График")]
        public int? CalendarId { get; set; } = null;
        public int? PersonId { get; set; }
        [DisplayName("Должность")]
        public int? PersonPositionId { get; set; }
        public int? UserId { get; set; }

        [DisplayName("Логин")]
        [LoginExists(ErrorMessage ="Пользователь с таким логином уже существует.")]
        public string Login { get; set; }
        [DisplayName("Пароль")]
        public string Password { get; set; }
        [DisplayName("Подтвердить пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
