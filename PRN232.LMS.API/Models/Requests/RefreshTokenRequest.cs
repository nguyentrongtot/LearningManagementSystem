using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class RefreshTokenRequest
{
    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string RefreshToken { get; set; } = null!;
}
