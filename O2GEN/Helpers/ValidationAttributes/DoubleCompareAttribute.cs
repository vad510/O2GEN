using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;

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
            var optionValue = (long?)optionProperty.GetValue(validationContext.ObjectInstance);

            try
            {
                switch (optionValue)
                {
                    case 2:
                        { }
                        break;
                    default:
                        {
                            if (double.Parse(currentValue, CultureInfo.InvariantCulture) <= double.Parse(toValidateWith, CultureInfo.InvariantCulture))
                                return new ValidationResult(ErrorMessage);
                        }
                        break;
                }
            }
            catch
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
