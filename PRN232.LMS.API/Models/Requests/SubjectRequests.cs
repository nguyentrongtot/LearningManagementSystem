using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class SubjectCreateRequest
{
    [Required]
    [StringLength(20, MinimumLength = 2)]
    [RegularExpression(@"^[A-Z]{3}\d{3}$", ErrorMessage = "Subject code must follow format like PRF192.")]
    public string SubjectCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string SubjectName { get; set; } = string.Empty;

    [Required]
    [Range(1, 10)]
    public int Credit { get; set; }
}

public class SubjectUpdateRequest
{
    [Required]
    [StringLength(20, MinimumLength = 2)]
    [RegularExpression(@"^[A-Z]{3}\d{3}$", ErrorMessage = "Subject code must follow format like PRF192.")]
    public string SubjectCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string SubjectName { get; set; } = string.Empty;

    [Required]
    [Range(1, 10)]
    public int Credit { get; set; }
}
