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
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly LmsDbContext _context;

        public EnrollmentRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Enrollment>> GetAllAsync(string? search, List<(string Field, bool IsDescending)> sortParams, int? page, int? size, bool includeRelations)
        {
            var query = _context.Enrollments.AsQueryable();

            if (includeRelations)
            {
                query = query.Include(e => e.Student).Include(e => e.Course).ThenInclude(c => c.Semester)
                             .Include(e => e.Course).ThenInclude(c => c.Subject);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e => e.Status.Contains(search) || (e.Student != null && e.Student.FullName.Contains(search)));
            }

            if (sortParams != null && sortParams.Any())
            {
                IOrderedQueryable<Enrollment>? orderedQuery = null;
                foreach (var param in sortParams)
                {
                    bool isThenBy = orderedQuery != null;
                    var currentQuery = orderedQuery ?? query;

                    orderedQuery = param.Field.ToLower() switch
                    {
                        "status" => param.IsDescending 
                            ? (isThenBy ? orderedQuery!.ThenByDescending(e => e.Status) : currentQuery.OrderByDescending(e => e.Status))
                            : (isThenBy ? orderedQuery!.ThenBy(e => e.Status) : currentQuery.OrderBy(e => e.Status)),
                        "enrolldate" => param.IsDescending 
                            ? (isThenBy ? orderedQuery!.ThenByDescending(e => e.EnrollDate) : currentQuery.OrderByDescending(e => e.EnrollDate))
                            : (isThenBy ? orderedQuery!.ThenBy(e => e.EnrollDate) : currentQuery.OrderBy(e => e.EnrollDate)),
                        _ => currentQuery.OrderBy(e => e.EnrollmentId)
                    };
                }
                query = orderedQuery ?? query;
            }

            page = (page < 1 || !page.HasValue) ? 1 : page;
            size = (size < 1 || !size.HasValue) ? 10 : size;

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page.Value - 1) * size.Value).Take(size.Value).ToListAsync();

            return new PagedResult<Enrollment>(items, page.Value, size.Value, totalCount);
        }

        public async Task<Enrollment?> GetByIdAsync(int id, bool includeRelations)
        {
            var query = _context.Enrollments.AsQueryable();
            if (includeRelations)
            {
                query = query.Include(e => e.Student).Include(e => e.Course).ThenInclude(c => c.Semester)
                             .Include(e => e.Course).ThenInclude(c => c.Subject);
            }
            return await query.FirstOrDefaultAsync(e => e.EnrollmentId == id);
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Enrollments.FindAsync(id);
            if (entity != null)
            {
                _context.Enrollments.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> HasEnrollmentsByStudentAsync(int studentId)
        {
            return await _context.Enrollments.AnyAsync(e => e.StudentId == studentId);
        }

        public async Task<bool> HasEnrollmentsByCourseAsync(int courseId)
        {
            return await _context.Enrollments.AnyAsync(e => e.CourseId == courseId);
        }
    }
}
