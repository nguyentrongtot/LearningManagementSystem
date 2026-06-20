using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Business;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Helpers
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings;
        public JwtTokenGenerator(JwtSettings jwtSettings)
        {
            _settings = jwtSettings;
        }
        public string GenerateToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public int GetExpiresInSeconds()
            => _settings.AccessTokenExpiryMinutes * 60;
    }
}
