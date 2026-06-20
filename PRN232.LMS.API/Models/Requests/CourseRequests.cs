using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class CourseCreateRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int SemesterId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int SubjectId { get; set; }
}

public class CourseUpdateRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int SemesterId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int SubjectId { get; set; }
}
