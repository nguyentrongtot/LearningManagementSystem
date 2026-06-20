using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class StudentUpdateRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    public DateTime DateOfBirth { get; set; }
}
