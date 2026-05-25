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
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<PagedResult<ExpandoObject>> GetEnrollmentsAsync(string? search, string? sort, int? page = 1, int? size = 10, string? fields = null, string? expand = null)
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

            bool includeRelations = !string.IsNullOrWhiteSpace(expand) && 
                (expand.Contains("student", StringComparison.OrdinalIgnoreCase) || expand.Contains("course", StringComparison.OrdinalIgnoreCase));

            var enrollments = await _enrollmentRepository.GetAllAsync(search, sortParams, page, size, includeRelations);
            
            var dtos = enrollments.Items.Select(e => new EnrollmentDTO
            {
                EnrollmentId = e.EnrollmentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status,
                CourseDTO = e.Course == null ? null : new CourseDTO
                {
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    SemesterId = e.Course.SemesterId,
                    SubjectId = e.Course.SubjectId
                }
                // Note: the original EnrollmentDTO doesn't have StudentDTO, but we can return dynamic fields via shaping anyway.
            }).ToList();

            var shaped = dtos.Select(dto => DataShaper.ShapeData(dto, fields)).ToList();

            return new PagedResult<ExpandoObject>(shaped, enrollments.Page, enrollments.PageSize, enrollments.TotalItems);
        }

        public async Task<EnrollmentDTO?> GetEnrollmentByIdAsync(int id)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id, true);
            if (enrollment == null) return null;

            return new EnrollmentDTO
            {
                EnrollmentId = enrollment.EnrollmentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status,
                CourseDTO = enrollment.Course == null ? null : new CourseDTO
                {
                    CourseId = enrollment.Course.CourseId,
                    CourseName = enrollment.Course.CourseName,
                    SemesterId = enrollment.Course.SemesterId,
                    SubjectId = enrollment.Course.SubjectId,
                    SemesterDTO = enrollment.Course.Semester == null ? null : new SemesterDTO
                    {
                        SemesterId = enrollment.Course.Semester.SemesterId,
                        SemesterName = enrollment.Course.Semester.SemesterName,
                        StartDate = enrollment.Course.Semester.StartDate,
                        EndDate = enrollment.Course.Semester.EndDate
                    },
                    SubjectDTO = enrollment.Course.Subject == null ? null : new SubjectDTO
                    {
                        SubjectId = enrollment.Course.Subject.SubjectId,
                        SubjectCode = enrollment.Course.Subject.SubjectCode,
                        SubjectName = enrollment.Course.Subject.SubjectName,
                        Credit = enrollment.Course.Subject.Credit
                    }
                }
            };
        }

        public async Task<EnrollmentDTO> CreateEnrollmentAsync(EnrollmentCreateRequest createRequest)
        {
            var validStatuses = new[] { "Active", "Dropped", "Completed" };
            if (!validStatuses.Contains(createRequest.Status))
            {
                throw new ArgumentException("Status must be one of: Active, Dropped, Completed.");
            }

            var entity = new Enrollment
            {
                StudentId = createRequest.StudentId,
                CourseId = createRequest.CourseId,
                EnrollDate = createRequest.EnrollDate,
                Status = createRequest.Status
            };

            var created = await _enrollmentRepository.CreateAsync(entity);
            return new EnrollmentDTO
            {
                EnrollmentId = created.EnrollmentId,
                CourseId = created.CourseId,
                EnrollDate = created.EnrollDate,
                Status = created.Status
            };
        }

        public async Task<EnrollmentDTO> UpdateEnrollmentAsync(int id, EnrollmentUpdateRequest updateRequest)
        {
            var entity = await _enrollmentRepository.GetByIdAsync(id, false);
            if (entity == null) throw new KeyNotFoundException($"Enrollment ID {id} not found.");

            var validStatuses = new[] { "Active", "Dropped", "Completed" };
            if (!validStatuses.Contains(updateRequest.Status))
            {
                throw new ArgumentException("Status must be one of: Active, Dropped, Completed.");
            }

            entity.StudentId = updateRequest.StudentId;
            entity.CourseId = updateRequest.CourseId;
            entity.EnrollDate = updateRequest.EnrollDate;
            entity.Status = updateRequest.Status;

            await _enrollmentRepository.UpdateAsync(entity);

            return new EnrollmentDTO
            {
                EnrollmentId = entity.EnrollmentId,
                CourseId = entity.CourseId,
                EnrollDate = entity.EnrollDate,
                Status = entity.Status
            };
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            return await _enrollmentRepository.DeleteAsync(id);
        }
    }
}
