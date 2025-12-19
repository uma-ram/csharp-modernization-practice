using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Extensions;

public static class ValidationExtensions
{
    public static (bool IsValid, List<string> Errors) Validate(this object obj)
    {
        var validationResult = new List<ValidationResult>();
        var validationContext = new ValidationContext(obj);

        bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);

        var errors = validationResult.Select(v => v.ErrorMessage ?? "Validation error").ToList();

        return (isValid, errors);
    }
}
