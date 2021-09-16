using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;

namespace O2GEN.Helpers.ValidationAttributes
{
    public class RequiedIfAttribute : ValidationAttribute
    {
        private readonly string _targetVariable;
        private readonly string _valueProperty;

        public RequiedIfAttribute(string TargetVariable, string Value)
        {
            _targetVariable = TargetVariable;
            _valueProperty = Value;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;

            var property = validationContext.ObjectType.GetProperty(_targetVariable);
            if (((int)property.GetValue(validationContext.ObjectInstance)).ToString() == _valueProperty)
            {
                var currentValue = (int?)value;
                if (value == null)
                {
                    return new ValidationResult(ErrorMessageString);
                }
            }
            return ValidationResult.Success;
        }
    }
}
