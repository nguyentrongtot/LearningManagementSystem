using System.Collections.Generic;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.EntityModel;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<PagedResult<Enrollment>> GetAllAsync(string? search, List<(string Field, bool IsDescending)> sortParams, int? page, int? size, bool includeStudent,bool includeCourse);
        Task<Enrollment?> GetByIdAsync(int id, bool includeRelations);
        Task<Enrollment> CreateAsync(Enrollment enrollment);
        Task<Enrollment> UpdateAsync(Enrollment enrollment);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasEnrollmentsByStudentAsync(int studentId);
        Task<bool> HasEnrollmentsByCourseAsync(int courseId);
    }
}
