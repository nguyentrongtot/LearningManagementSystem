using System.Collections.Generic;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.EntityModel;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<PagedResult<Course>> GetAllAsync(string? search, List<(string Field, bool IsDescending)> sortParams, int? page, int? size, bool includeSemester,bool includeSubject);
        Task<Course?> GetByIdAsync(int id, bool includeRelations);
        Task<Course> CreateAsync(Course course);
        Task<Course> UpdateAsync(Course course);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasCoursesBySemesterAsync(int semesterId);
        Task<bool> HasCoursesBySubjectAsync(int subjectId);
    }
}
