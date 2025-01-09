using System;
using System.ComponentModel.DataAnnotations;

public class NotDefaultDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime && dateTime == DateTime.MinValue)
        {
            return new ValidationResult(ErrorMessage ?? "Date cannot be the default value.");
        }
        return ValidationResult.Success;
    }
}
