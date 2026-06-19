using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Business;

namespace PRN232.LMS.Services.BLL
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository , IStudentRepository studentRepository, ICourseRepository courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _courseRepository  = courseRepository;
        }

        public async Task<PagedResult<ExpandoObject>> GetEnrollmentsAsync(string? search, string? sort, int? page = 1, int? size = 10, string? fields = null, string? expand = null)
        {
            var sortParams = new List<(string Field, bool IsDescending)>();
            if (!string.IsNullOrWhiteSpace(sort))
            {
                foreach (var expression in sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var trimmedExpression = expression.Trim().ToLowerInvariant();
                    var isDescending = trimmedExpression.StartsWith("-");
                    var propertyName = isDescending ? trimmedExpression[1..] : trimmedExpression;
                    sortParams.Add((propertyName, isDescending));
                }
            }

            var expandFields = (expand ?? string.Empty)
                                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                .Select(x => x.ToLowerInvariant())
                                .ToHashSet();

            bool includeStudent = expandFields.Contains("student");
            bool includeCourse = expandFields.Contains("course");

            var enrollments = await _enrollmentRepository.GetAllAsync(search, sortParams, page, size, includeStudent, includeCourse);
            
            var dtos = enrollments.Items.Select(e => new EnrollmentDTO
            {
                EnrollmentId = e.EnrollmentId,
                CourseId = e.CourseId,
                StudentId = e.StudentId,
                EnrollDate = e.EnrollDate,
                Status = e.Status,
                CourseDTO = e.Course == null ? null : new CourseDTO
                {
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    SemesterId = e.Course.SemesterId,
                    SubjectId = e.Course.SubjectId,
                    SemesterDTO = e.Course.Semester == null ? null : new SemesterDTO
                    {
                        SemesterId = e.Course.Semester.SemesterId,
                        SemesterName = e.Course.Semester.SemesterName,
                        StartDate = e.Course.Semester.StartDate,
                        EndDate = e.Course.Semester.EndDate
                    },
                    SubjectDTO = e.Course.Subject == null ? null : new SubjectDTO
                    {
                        SubjectId = e.Course.Subject.SubjectId,
                        SubjectCode = e.Course.Subject.SubjectCode,
                        SubjectName = e.Course.Subject.SubjectName,
                        Credit = e.Course.Subject.Credit
                    }
                },
                StudentDTO = e.Student == null ? null : new StudentDTO
                {
                    StudentId = e.Student.StudentId,
                    FullName = e.Student.FullName,
                    DateOfBirth = e.Student.DateOfBirth,
                    Email = e.Student.Email
                }
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
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status,
                StudentDTO = enrollment.Student == null ? null : new StudentDTO
                {
                    StudentId = enrollment.Student.StudentId,
                    FullName = enrollment.Student.FullName,
                    Email = enrollment.Student.Email,
                    DateOfBirth = enrollment.Student.DateOfBirth
                },
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
            if(await _studentRepository.GetByIdAsync(createRequest.StudentId) == null)
            {
                throw new ArgumentException("Student is not exist");
            }

            if (await _courseRepository.GetByIdAsync(createRequest.CourseId) == null)
            {
                throw new ArgumentException("Course is not exist");
            }
            var entity = new Enrollment
            {
                StudentId = createRequest.StudentId,
                CourseId = createRequest.CourseId,
                EnrollDate = createRequest.EnrollDate,
                Status = createRequest.Status
            };

            var created = await _enrollmentRepository.CreateAsync(entity);
            var student = await _studentRepository.GetByIdAsync(createRequest.StudentId);
            var course = await _courseRepository.GetByIdAsync(createRequest.CourseId);

            return new EnrollmentDTO
            {
                EnrollmentId = created.EnrollmentId,
                StudentId = created.StudentId,
                CourseId = created.CourseId,
                EnrollDate = created.EnrollDate,
                Status = created.Status,
                StudentDTO = student == null ? null : new StudentDTO
                {
                    StudentId = student.StudentId,
                    FullName = student.FullName,
                    Email = student.Email,
                    DateOfBirth = student.DateOfBirth
                },
                CourseDTO = course == null ? null : new CourseDTO
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
                }
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
