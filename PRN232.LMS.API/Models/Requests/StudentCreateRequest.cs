using System.ComponentModel.DataAnnotations;
using PRN232.LMS.API.Validation;

namespace PRN232.LMS.API.Models.Requests;

public class StudentCreateRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    [FptuStudentCode]
    public string StudentCode { get; set; } = null!;

    [Required]
    public DateTime DateOfBirth { get; set; }
}
