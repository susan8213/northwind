using System.ComponentModel.DataAnnotations;

namespace Northwind.Attributes;

public class FromNowAttribute : ValidationAttribute
{
    public string GetErrorMessage() => "Date must be past now";
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (((DateTime?)value) == null) return ValidationResult.Success;

        var date = (DateTime)value;
        if (DateTime.Compare(date, DateTime.Now) < 0) return new ValidationResult(GetErrorMessage());
        else return ValidationResult.Success;
    }
}