using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(RequestMapper.ToBusiness(request));
                return Ok(ApiResponseFactory.Success(ResponseMapper.ToResponse(result)));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiErrorResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = null
                });
            }
        }
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(RequestMapper.ToBusiness(request));
                return Ok(ApiResponseFactory.Success(ResponseMapper.ToResponse(result)));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiErrorResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = null
                });
            }
        }
    }
}
