using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Business;

namespace PRN232.LMS.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PagedResult<ExpandoObject>> GetCoursesAsync(string? search, string? sort, int? page, int? size, string? fields, string? expand);
        Task<CourseDTO?> GetCourseByIdAsync(int id);
        Task<CourseDTO> CreateCourseAsync(CourseCreateRequest createRequest);
        Task<CourseDTO> UpdateCourseAsync(int id, CourseUpdateRequest updateRequest);
        Task<bool> DeleteCourseAsync(int id);
        Task<IEnumerable<EnrollmentDTO>> GetEnrollmentsByCourseAsync(int courseId, string? expand);
    }
}
