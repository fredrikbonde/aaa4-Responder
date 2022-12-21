using System;
using System.ComponentModel.DataAnnotations;

namespace aaa3.basic.WebApi.Attributes
{
    public class DateLessThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateLessThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var currentValue = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
            {
                throw new ArgumentException($"Property with name {_comparisonProperty} not found");
            }
            var comparissonProperty = property.GetValue(validationContext.ObjectInstance);
            if (comparissonProperty == null)
            {
                return ValidationResult.Success;
            }
            var comparisonValue = (DateTime)comparissonProperty;

            if (currentValue > comparisonValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}