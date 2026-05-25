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
    public class CourseRepository : ICourseRepository
    {
        private readonly LmsDbContext _context;

        public CourseRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Course>> GetAllAsync(string? search, List<(string Field, bool IsDescending)> sortParams, int? page, int? size, bool includeRelations)
        {
            var query = _context.Courses.AsQueryable();

            if (includeRelations)
            {
                query = query.Include(c => c.Semester).Include(c => c.Subject);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.CourseName.Contains(search));
            }

            if (sortParams != null && sortParams.Any())
            {
                IOrderedQueryable<Course>? orderedQuery = null;
                foreach (var param in sortParams)
                {
                    bool isThenBy = orderedQuery != null;
                    var currentQuery = orderedQuery ?? query;

                    orderedQuery = param.Field.ToLower() switch
                    {
                        "coursename" => param.IsDescending 
                            ? (isThenBy ? orderedQuery!.ThenByDescending(c => c.CourseName) : currentQuery.OrderByDescending(c => c.CourseName))
                            : (isThenBy ? orderedQuery!.ThenBy(c => c.CourseName) : currentQuery.OrderBy(c => c.CourseName)),
                        _ => currentQuery.OrderBy(c => c.CourseId)
                    };
                }
                query = orderedQuery ?? query;
            }

            page = (page < 1 || !page.HasValue) ? 1 : page;
            size = (size < 1 || !size.HasValue) ? 10 : size;

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page.Value - 1) * size.Value).Take(size.Value).ToListAsync();

            return new PagedResult<Course>(items, page.Value, size.Value, totalCount);
        }

        public async Task<Course?> GetByIdAsync(int id, bool includeRelations)
        {
            var query = _context.Courses.AsQueryable();
            if (includeRelations)
            {
                query = query.Include(c => c.Semester).Include(c => c.Subject);
            }
            return await query.FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<Course> CreateAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Courses.FindAsync(id);
            if (entity != null)
            {
                _context.Courses.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> HasCoursesBySemesterAsync(int semesterId)
        {
            return await _context.Courses.AnyAsync(c => c.SemesterId == semesterId);
        }

        public async Task<bool> HasCoursesBySubjectAsync(int subjectId)
        {
            return await _context.Courses.AnyAsync(c => c.SubjectId == subjectId);
        }
    }
}
