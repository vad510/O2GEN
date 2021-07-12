using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace O2GEN.Helpers.ValidationAttributes
{
    public class DoubleCompareAttribute : ValidationAttribute
    {
        private readonly string _toCompareWith;

        public DoubleCompareAttribute(string toCompare)
        {
            _toCompareWith = toCompare;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;

            var currentValue = (string)value;

            var property = validationContext.ObjectType.GetProperty(_toCompareWith);

            if (property == null)
            {
                Debug.WriteLine($"No such property in {validationContext.DisplayName}");
                return new ValidationResult("Unexpected error");
            }

            var toValidateWith = (string)property.GetValue(validationContext.ObjectInstance);

            if (double.Parse(currentValue.Replace(",", ".")) >= double.Parse(toValidateWith.Replace(",", ".")))
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
