using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.DAL
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly LmsDbContext _context;
        public RefreshTokenRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAsync(string token)
        {
            var entity = await GetByTokenAsync(token);
            if (entity is null) return;

            entity.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }
}
