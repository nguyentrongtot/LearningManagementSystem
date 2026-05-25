using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.EntityModel;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Implementations
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly LmsDbContext _context;

        public SemesterRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Semester>> GetAllAsync(string? search, List<(string Field, bool IsDescending)> sortParams, int? page, int? size)
        {
            var query = _context.Semesters.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.SemesterName.Contains(search));
            }

            if (sortParams != null && sortParams.Any())
            {
                IOrderedQueryable<Semester>? orderedQuery = null;
                foreach (var param in sortParams)
                {
                    bool isThenBy = orderedQuery != null;
                    var currentQuery = orderedQuery ?? query;

                    orderedQuery = param.Field.ToLower() switch
                    {
                        "semestername" => param.IsDescending 
                            ? (isThenBy ? orderedQuery!.ThenByDescending(s => s.SemesterName) : currentQuery.OrderByDescending(s => s.SemesterName))
                            : (isThenBy ? orderedQuery!.ThenBy(s => s.SemesterName) : currentQuery.OrderBy(s => s.SemesterName)),
                        "startdate" => param.IsDescending 
                            ? (isThenBy ? orderedQuery!.ThenByDescending(s => s.StartDate) : currentQuery.OrderByDescending(s => s.StartDate))
                            : (isThenBy ? orderedQuery!.ThenBy(s => s.StartDate) : currentQuery.OrderBy(s => s.StartDate)),
                        "enddate" => param.IsDescending 
                            ? (isThenBy ? orderedQuery!.ThenByDescending(s => s.EndDate) : currentQuery.OrderByDescending(s => s.EndDate))
                            : (isThenBy ? orderedQuery!.ThenBy(s => s.EndDate) : currentQuery.OrderBy(s => s.EndDate)),
                        _ => currentQuery.OrderBy(s => s.SemesterId)
                    };
                }
                query = orderedQuery ?? query;
            }

            page = (page < 1 || !page.HasValue) ? 1 : page;
            size = (size < 1 || !size.HasValue) ? 10 : size;

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page.Value - 1) * size.Value).Take(size.Value).ToListAsync();

            return new PagedResult<Semester>(items, page.Value, size.Value, totalCount);
        }

        public async Task<Semester?> GetByIdAsync(int id)
        {
            return await _context.Semesters.FindAsync(id);
        }

        public async Task<Semester> CreateAsync(Semester semester)
        {
            await _context.Semesters.AddAsync(semester);
            await _context.SaveChangesAsync();
            return semester;
        }

        public async Task<Semester> UpdateAsync(Semester semester)
        {
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();
            return semester;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Semesters.FindAsync(id);
            if (entity != null)
            {
                _context.Semesters.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
