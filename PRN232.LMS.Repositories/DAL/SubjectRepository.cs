using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.DAL
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly LmsDbContext _context;

        public SubjectRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Subject>> GetAllAsync(string? search, List<(string Field, bool IsDescending)> sortParams, int? page, int? size)
        {
            var query = _context.Subjects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.SubjectCode.Contains(search) || s.SubjectName.Contains(search));
            }

            if (sortParams != null && sortParams.Any())
            {
                IOrderedQueryable<Subject>? orderedQuery = null;
                foreach (var param in sortParams)
                {
                    bool isThenBy = orderedQuery != null;
                    var currentQuery = orderedQuery ?? query;

                    orderedQuery = param.Field.ToLower() switch
                    {
                        "subjectcode" => param.IsDescending 
                            ? (isThenBy ? orderedQuery!.ThenByDescending(s => s.SubjectCode) : currentQuery.OrderByDescending(s => s.SubjectCode))
                            : (isThenBy ? orderedQuery!.ThenBy(s => s.SubjectCode) : currentQuery.OrderBy(s => s.SubjectCode)),
                        "subjectname" => param.IsDescending 
                            ? (isThenBy ? orderedQuery!.ThenByDescending(s => s.SubjectName) : currentQuery.OrderByDescending(s => s.SubjectName))
                            : (isThenBy ? orderedQuery!.ThenBy(s => s.SubjectName) : currentQuery.OrderBy(s => s.SubjectName)),
                        "credit" => param.IsDescending 
                            ? (isThenBy ? orderedQuery!.ThenByDescending(s => s.Credit) : currentQuery.OrderByDescending(s => s.Credit))
                            : (isThenBy ? orderedQuery!.ThenBy(s => s.Credit) : currentQuery.OrderBy(s => s.Credit)),
                        _ => currentQuery.OrderBy(s => s.SubjectId)
                    };
                }
                query = orderedQuery ?? query;
            }

            page = (page < 1 || !page.HasValue) ? 1 : page;
            size = (size < 1 || !size.HasValue) ? 10 : size;

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page.Value - 1) * size.Value).Take(size.Value).ToListAsync();

            return new PagedResult<Subject>(items, page.Value, size.Value, totalCount);
        }

        public async Task<Subject?> GetByIdAsync(int id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public async Task<bool> IsCodeExistAsync(string code)
        {
            return await _context.Subjects.AnyAsync(s => s.SubjectCode == code.Trim());
        }

        public async Task<Subject> CreateAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<Subject> UpdateAsync(Subject subject)
        {
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Subjects.FindAsync(id);
            if (entity != null)
            {
                _context.Subjects.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
