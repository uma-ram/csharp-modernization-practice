namespace TodoApi.Extensions;

using System.ComponentModel.DataAnnotations;

public static class ValidationExtensions
{
    public static (bool IsValid, List<string> Errors) Validate(this object obj)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(obj);

        bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

        var errors = validationResults.Select(v => v.ErrorMessage ?? "Validation error").ToList();

        return (isValid, errors);
    }
}