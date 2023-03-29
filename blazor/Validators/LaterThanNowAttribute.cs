using System.ComponentModel.DataAnnotations;

namespace blazor.Validators;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class LaterThanNowAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        bool isDate = DateTime.TryParse(value?.ToString(), out DateTime result);
        if (!isDate)
        {
            throw new ArgumentException("Value is not a date");
        }

        if (result <= DateTime.Now)
        {
            return new ValidationResult("Date should be later than now");
        }

        return null;
    }
}
