using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace O2GEN.Helpers.ValidationAttributes
{
    /// <summary>
    /// Проверяет логин на существование
    /// Только для <see cref="O2GEN.Models.Engineer"/>
    /// </summary>
    public class LoginExistsAttribute : ValidationAttribute
    {

        public LoginExistsAttribute()
        {}

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string login = value.ToString();
            int Id = 0;
            int.TryParse(validationContext.ObjectType.GetProperty("UserId").GetValue(validationContext.ObjectInstance)?.ToString(), out Id);
            
            if(Id == 0)
            {
                if(DBHelper.LoginIsExist(login, null) != "-1")
                {
                    return new ValidationResult(ErrorMessageString);
                }
            }

            return ValidationResult.Success;
        }
    }
}
