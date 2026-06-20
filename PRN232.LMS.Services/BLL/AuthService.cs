using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.DAL;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Business;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.BLL
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenRepository refreshTokenRepository,
            JwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtSettings = jwtSettings;
        }
        public async Task<AuthTokenDTO> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if(user == null  || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
            var accessToken = _jwtTokenGenerator.GenerateToken(user);
            var refreshToken = new RefreshToken
            {
                UserId = user.UserId,
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(refreshToken);

            return new AuthTokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = _jwtTokenGenerator.GetExpiresInSeconds(),
                User = new UserDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Role = user.Role
                }
            };
        }

        public async Task<AuthTokenDTO> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            var user = storedToken.User;
            await _refreshTokenRepository.RevokeAsync(storedToken.Token);

            var accessToken = _jwtTokenGenerator.GenerateToken(user);
            var refreshToken = new RefreshToken
            {
                UserId = user.UserId,
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(refreshToken);

            return new AuthTokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = _jwtTokenGenerator.GetExpiresInSeconds(),
                User = new UserDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Role = user.Role
                }
            };
        }
    }
}
