using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace VolHub.Mvc.Models.Validation;

/// <summary>
/// Validates that a password matches the laboratory requirements.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PasswordComplexityAttribute : ValidationAttribute, IClientModelValidator
{
    public int MinimumLength { get; init; } = 8;
    public int MaximumLength { get; init; } = 16;

    private const string ClientRegexPattern = @"^(?=.*\d)(?=.*[A-Z])(?=.*[^A-Za-z0-9]).{8,16}$";

    public PasswordComplexityAttribute()
        : base("Пароль має містити від 8 до 16 символів, щонайменше одну цифру, один спеціальний символ та одну велику літеру.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string password || string.IsNullOrWhiteSpace(password))
        {
            // [Required] handles empty values.
            return ValidationResult.Success;
        }

        if (password.Length < MinimumLength || password.Length > MaximumLength)
        {
            return new ValidationResult(ErrorMessage);
        }

        if (!password.Any(char.IsDigit))
        {
            return new ValidationResult(ErrorMessage);
        }

        if (!password.Any(char.IsUpper))
        {
            return new ValidationResult(ErrorMessage);
        }

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-passwordcomplexity", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
        MergeAttribute(context.Attributes, "data-val-passwordcomplexity-min", MinimumLength.ToString());
        MergeAttribute(context.Attributes, "data-val-passwordcomplexity-max", MaximumLength.ToString());
        MergeAttribute(context.Attributes, "data-val-regex", ErrorMessage ?? FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
        MergeAttribute(context.Attributes, "data-val-regex-pattern", ClientRegexPattern);
    }

    private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key))
        {
            attributes[key] = value;
        }
    }
}
