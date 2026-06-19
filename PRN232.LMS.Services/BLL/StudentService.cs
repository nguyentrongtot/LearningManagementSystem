using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Core;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Business;

namespace PRN232.LMS.Services.BLL
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        
        public StudentService(IStudentRepository studentRepository, IEnrollmentRepository enrollmentRepository)
        {
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<StudentDTO> CreateStudentAsync(StudentCreateRequest createRequest)
        {
            if (!IsValidEmail(createRequest.Email))
            {
                throw new ArgumentException("Email không đúng định dạng FPT (Ví dụ chuẩn: totntse181863@fpt.edu.vn)");
            }

            if (await _studentRepository.IsEmailExistAsync(createRequest.Email))
            {
                throw new ArgumentException("Email này đã tồn tại trong hệ thống. Vui lòng sử dụng email khác.");
            }
            
            var studentEntity = new Student
            {
                FullName = createRequest.FullName,
                Email = createRequest.Email,
                DateOfBirth = createRequest.DateOfBirth
            };

            var studentCreated = await _studentRepository.CreateAsync(studentEntity);

            return new StudentDTO
            {
                StudentId = studentCreated.StudentId,
                FullName = studentCreated.FullName,
                Email = studentCreated.Email,
                DateOfBirth = studentCreated.DateOfBirth
            };
        }

        public async Task<PagedResult<ExpandoObject>> GetStudentAsync(string? searchTerm, string? sort, int? page = 1, int? size = 10, string? fields = null, string? expand=null)
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
            
            bool includeEnrollments = !string.IsNullOrWhiteSpace(expand) && expand.Contains("enrollment", StringComparison.OrdinalIgnoreCase);

            var students = await _studentRepository.GetAllAsync(searchTerm, sortParams, page, size, includeEnrollments);
            var studentDtos = students.Items.Select(s => new StudentDTO
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                DateOfBirth = s.DateOfBirth,
                EnrollmentDTOs = s.Enrollments.Select(e => new EnrollmentDTO
                {
                    EnrollmentId = e.EnrollmentId,
                    //StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status,
                    CourseDTO = null
                }).ToList()
            }).ToList();
            var shapedStudents = studentDtos.Select(dto => DataShaper.ShapeData(dto, fields)).ToList();

            return new PagedResult<ExpandoObject>(shapedStudents, students.Page, students.PageSize, students.TotalItems);
        }

        public async Task<StudentDTO?> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null) return null;
            var studentResponse = new StudentDTO
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                EnrollmentDTOs = student.Enrollments.Select(e => new EnrollmentDTO
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
                    }
                }).ToList()
            };
            return studentResponse;
        }

        public async Task<StudentDTO> UpdateStudentAsync(int id,StudentUpdateRequest studentUpdateRequest)
        {
            var studentEntity = await _studentRepository.GetByIdAsync(id);
            if (studentEntity != null)
            {
                if (!IsValidEmail(studentUpdateRequest.Email))
                {
                    throw new ArgumentException("Email không đúng định dạng FPT (Ví dụ chuẩn: totntse181863@fpt.edu.vn)");
                }
                studentEntity.FullName = studentUpdateRequest.FullName;
                studentEntity.Email = studentUpdateRequest.Email;
                studentEntity.DateOfBirth = studentUpdateRequest.DateOfBirth;

                await _studentRepository.UpdateAsync(studentEntity);

                return new StudentDTO
                {
                    StudentId = studentEntity.StudentId,
                    FullName = studentEntity.FullName,
                    Email = studentEntity.Email,
                    DateOfBirth = studentEntity.DateOfBirth
                };
            }
            else
            {
                throw new KeyNotFoundException($"Không tìm thấy sinh viên với ID: {id}");
            }
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            string emailPattern = @"^[a-z]+se\d{6}@fpt\.edu\.vn$";
            return Regex.IsMatch(email.Trim().ToLower(), emailPattern);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            if (await _enrollmentRepository.HasEnrollmentsByStudentAsync(id))
            {
                throw new InvalidOperationException($"Không thể xóa sinh viên với ID {id} vì sinh viên này đã có dữ liệu đăng ký khóa học (Enrollments).");
            }
            return await _studentRepository.DeleteAsync(id);
        }
    }
}
