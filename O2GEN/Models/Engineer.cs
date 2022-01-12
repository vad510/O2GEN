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
        public long Id { get; set; } = -1;
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
        public long? DepartmentId { get; set; }
        [DisplayName("График")]
        public long? CalendarId { get; set; } = null;
        public long? PersonId { get; set; }
        [DisplayName("Должность")]
        public long? PersonPositionId { get; set; }
        public long? UserId { get; set; }

        [DisplayName("Логин")]
        [LoginExists(ErrorMessage ="Пользователь с таким логином уже существует.")]
        public string Login { get; set; }
        [DisplayName("Пароль")]
        public string Password { get; set; }
        [DisplayName("Подтвердить пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        [DisplayName("Создан")]
        public DateTime CreateStamp { get; set; }
        [DisplayName("Редактирован")]
        public DateTime ModifyStamp { get; set; }
    }
}
