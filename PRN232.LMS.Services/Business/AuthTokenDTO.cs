namespace PRN232.LMS.Services.Business;

public class AuthTokenDTO
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int ExpiresIn { get; set; }
    public UserDTO? User { get; set; }
}
