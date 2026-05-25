using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.EntityModel;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public CourseService(ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository)
        {
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<PagedResult<ExpandoObject>> GetCoursesAsync(string? search, string? sort, int? page = 1, int? size = 10, string? fields = null, string? expand = null)
        {
            var sortParams = new List<(string Field, bool IsDescending)>();
            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortExpression = sort.Split(',');
                foreach (var expression in sortExpression)
                {
                    var trimmedExpression = expression.Trim().ToLower();
                    var isDescending = trimmedExpression.StartsWith("-");
                    var propertyName = isDescending ? trimmedExpression.Substring(1) : trimmedExpression;
                    sortParams.Add((propertyName, isDescending));
                }
            }

            //bool includeRelations = !string.IsNullOrWhiteSpace(expand) && 
            //    (expand.Contains("semester", StringComparison.OrdinalIgnoreCase) || expand.Contains("subject", StringComparison.OrdinalIgnoreCase));

            var expandFields = expand?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToLowerInvariant())
            .ToHashSet() ?? new HashSet<string>();

            bool includeSemester = expandFields.Contains("semester");
            bool includeSubject = expandFields.Contains("subject");

            var courses = await _courseRepository.GetAllAsync(search, sortParams, page, size, includeSemester,includeSubject);
            
            var dtos = courses.Items.Select(c => new CourseDTO
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                SemesterId = c.SemesterId,
                SubjectId = c.SubjectId,
                SemesterDTO =!includeSemester ? null : new SemesterDTO
                {
                    SemesterId = c.Semester.SemesterId,
                    SemesterName = c.Semester.SemesterName,
                    StartDate = c.Semester.StartDate,
                    EndDate = c.Semester.EndDate
                },
                SubjectDTO = !includeSubject ? null : new SubjectDTO
                {   
                    SubjectId = c.Subject.SubjectId,
                    SubjectCode = c.Subject.SubjectCode,
                    SubjectName = c.Subject.SubjectName,
                    Credit = c.Subject.Credit
                }
            }).ToList();

            var shaped = dtos.Select(dto => DataShaper.ShapeData(dto, fields)).ToList();

            return new PagedResult<ExpandoObject>(shaped, courses.Page, courses.PageSize, courses.TotalItems);
        }

        public async Task<CourseDTO?> GetCourseByIdAsync(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id, true);
            if (course == null) return null;

            return new CourseDTO
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId,
                SubjectId = course.SubjectId,
                SemesterDTO = course.Semester == null ? null : new SemesterDTO
                {
                    SemesterId = course.Semester.SemesterId,
                    SemesterName = course.Semester.SemesterName,
                    StartDate = course.Semester.StartDate,
                    EndDate = course.Semester.EndDate
                },
                SubjectDTO = course.Subject == null ? null : new SubjectDTO
                {
                    SubjectId = course.Subject.SubjectId,
                    SubjectCode = course.Subject.SubjectCode,
                    SubjectName = course.Subject.SubjectName,
                    Credit = course.Subject.Credit
                }
            };
        }

        public async Task<CourseDTO> CreateCourseAsync(CourseCreateRequest createRequest)
        {
            var entity = new Course
            {
                CourseName = createRequest.CourseName,
                SemesterId = createRequest.SemesterId,
                SubjectId = createRequest.SubjectId
            };

            var created = await _courseRepository.CreateAsync(entity);
            return new CourseDTO
            {
                CourseId = created.CourseId,
                CourseName = created.CourseName,
                SemesterId = created.SemesterId,
                SubjectId = created.SubjectId
            };
        }

        public async Task<CourseDTO> UpdateCourseAsync(int id, CourseUpdateRequest updateRequest)
        {
            var entity = await _courseRepository.GetByIdAsync(id, false);
            if (entity == null) throw new KeyNotFoundException($"Course ID {id} not found.");

            entity.CourseName = updateRequest.CourseName;
            entity.SemesterId = updateRequest.SemesterId;
            entity.SubjectId = updateRequest.SubjectId;

            await _courseRepository.UpdateAsync(entity);

            return new CourseDTO
            {
                CourseId = entity.CourseId,
                CourseName = entity.CourseName,
                SemesterId = entity.SemesterId,
                SubjectId = entity.SubjectId
            };
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            if (await _enrollmentRepository.HasEnrollmentsByCourseAsync(id))
            {
                throw new InvalidOperationException($"Không thể xóa khóa học với ID {id} vì khóa học này đã có sinh viên đăng ký (Enrollments).");
            }
            return await _courseRepository.DeleteAsync(id);
        }
    }
}
