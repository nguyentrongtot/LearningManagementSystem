using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class EnrollmentCreateRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    [Required]
    public DateTime EnrollDate { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression("^(Active|Dropped|Completed)$", ErrorMessage = "Status must be Active, Dropped, or Completed.")]
    public string Status { get; set; } = string.Empty;
}

public class EnrollmentUpdateRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    [Required]
    public DateTime EnrollDate { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression("^(Active|Dropped|Completed)$", ErrorMessage = "Status must be Active, Dropped, or Completed.")]
    public string Status { get; set; } = string.Empty;
}
