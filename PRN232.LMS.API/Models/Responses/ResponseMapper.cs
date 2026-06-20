using PRN232.LMS.Services.Business;

namespace PRN232.LMS.API.Models.Responses;

public static class ResponseMapper
{
    public static AuthTokenResponse ToResponse(AuthTokenDTO dto) => new()
    {
        AccessToken = dto.AccessToken,
        RefreshToken = dto.RefreshToken,
        ExpiresIn = dto.ExpiresIn
    };

    public static UserResponse ToResponse(UserDTO dto) => new()
    {
        UserId = dto.UserId,
        Username = dto.Username,
        Role = dto.Role
    };
}
