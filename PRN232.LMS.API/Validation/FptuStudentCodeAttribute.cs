using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PRN232.LMS.API.Validation;

public partial class FptuStudentCodeAttribute : ValidationAttribute
{
    [GeneratedRegex(@"^(SE|CE|SS|SA|HS)\d{5}$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex StudentCodePattern();

    public FptuStudentCodeAttribute()
    {
        ErrorMessage = "Student code must follow FPTU format (e.g. SE19886, CE18793).";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string code || string.IsNullOrWhiteSpace(code))
        {
            return new ValidationResult("Student code is required.");
        }

        return StudentCodePattern().IsMatch(code)
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage);
    }
}
