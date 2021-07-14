using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace O2GEN.Helpers.ValidationAttributes
{
    public class DoubleCompareAttribute : ValidationAttribute
    {
        private readonly string _toCompareWith;
        private readonly string _optionProperty;

        public DoubleCompareAttribute(string toCompare, string option)
        {
            _toCompareWith = toCompare;
            _optionProperty = option;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;

            var currentValue = (string)value;

            var property = validationContext.ObjectType.GetProperty(_toCompareWith);
            var optionProperty = validationContext.ObjectType.GetProperty(_optionProperty);

            if (property == null)
            {
                Debug.WriteLine($"No such property in {validationContext.DisplayName}");
                return new ValidationResult("Unexpected error");
            }

            var toValidateWith = (string)property.GetValue(validationContext.ObjectInstance);
            var optionValue = (int?)optionProperty.GetValue(validationContext.ObjectInstance);

            switch (optionValue)
            {
                case 0:
                    case 2:
                    {
                        if (double.Parse(currentValue.Replace(",", ".")) <= double.Parse(toValidateWith.Replace(",", ".")))
                            return new ValidationResult(ErrorMessage);
                    }
                    break;
                default:
                    break;
            }

            return ValidationResult.Success;
        }
    }
}
