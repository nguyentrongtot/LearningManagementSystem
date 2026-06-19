using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class StudentCreateRequest
{
    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public DateTime DateOfBirth { get; set; }
}
