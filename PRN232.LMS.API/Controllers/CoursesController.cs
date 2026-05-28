using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<System.Dynamic.ExpandoObject>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? fields, [FromQuery] string? expand)
        {
            var pagedResult = await _courseService.GetCoursesAsync(search, sort, page, size, fields, expand);
            if (pagedResult == null || pagedResult.Items.Count == 0)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Message = "No courses found.",
                    Errors = new List<string> { "No courses available with the given criteria." }
                });
            }

            return Ok(new ApiResponse<List<System.Dynamic.ExpandoObject>>
            {
                Success = true,
                Message = "Courses retrieved successfully.",
                Data = pagedResult.Items,
                Errors = null,
                Pagination = new PaginationMetadata
                {
                    Page = pagedResult.Page,
                    PageSize = pagedResult.PageSize,
                    TotalItems = pagedResult.TotalItems,
                    TotalPages = pagedResult.TotalPages
                }
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Message = $"Course with ID {id} not found.",
                    Errors = new List<string> { $"No course found with ID {id}." }
                });
            }

            var responseData = new CourseResponse
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId,
                SubjectId = course.SubjectId,
                Semester = course.SemesterDTO == null ? null : new SemesterResponse
                {
                    SemesterId = course.SemesterDTO.SemesterId,
                    SemesterName = course.SemesterDTO.SemesterName,
                    StartDate = course.SemesterDTO.StartDate,
                    EndDate = course.SemesterDTO.EndDate
                },
                Subject = course.SubjectDTO == null ? null : new SubjectResponse
                {
                    SubjectId = course.SubjectDTO.SubjectId,
                    SubjectCode = course.SubjectDTO.SubjectCode,
                    SubjectName = course.SubjectDTO.SubjectName,
                    Credit = course.SubjectDTO.Credit
                }
            };

            return Ok(new ApiResponse<CourseResponse>
            {
                Success = true,
                Message = $"Course with ID {id} retrieved successfully.",
                Data = responseData,
                Errors = null
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CourseCreateRequest createRequest)
        {
            try
            {
                var created = await _courseService.CreateCourseAsync(createRequest);
                var responseData = new CourseResponse
                {
                    CourseId = created.CourseId,
                    CourseName = created.CourseName,
                    SemesterId = created.SemesterId,
                    SubjectId = created.SubjectId
                };

                return CreatedAtAction(nameof(GetById), new { id = created.CourseId }, new ApiResponse<CourseResponse>
                {
                    Success = true,
                    Message = "Course created successfully.",
                    Data = responseData,
                    Errors = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Message = "Failed to create course.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] CourseUpdateRequest updateRequest)
        {
            try
            {
                var updated = await _courseService.UpdateCourseAsync(id, updateRequest);
                var responseData = new CourseResponse
                {
                    CourseId = updated.CourseId,
                    CourseName = updated.CourseName,
                    SemesterId = updated.SemesterId,
                    SubjectId = updated.SubjectId
                };

                return Ok(new ApiResponse<CourseResponse>
                {
                    Success = true,
                    Message = $"Course with ID {id} updated successfully.",
                    Data = responseData,
                    Errors = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Message = "Failed to update course.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var isDeleted = await _courseService.DeleteCourseAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        Message = $"Course with ID {id} not found.",
                        Errors = new List<string> { $"No course found with ID {id}." }
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = $"Course with ID {id} deleted successfully.",
                    Data = null,
                    Errors = null
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Message = "Cannot delete course.",
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting the course.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 2. ENDPOINT MỚI: GET /api/courses/{id}/enrollments
        [HttpGet("{id}/enrollments")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EnrollmentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEnrollmentsByCourse(int id, [FromQuery] string? expand = null)
        {
            // Bước A: Kiểm tra xem Khóa học (Course) này có tồn tại không trước khi lấy Enrollments
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Message = $"Course with ID {id} not found.",
                    Errors = new List<string> { $"Cannot retrieve enrollments because course ID {id} does not exist." }
                });
            }

            // Bước B: Gọi Service lấy danh sách DTOs
            var enrollmentDtos = await _courseService.GetEnrollmentsByCourseAsync(id, expand);

            // Bước C: Mapping từ DTO sang Response hiển thị
            var responseData = enrollmentDtos.Select(e => new EnrollmentInCourseResponse
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = id,
                EnrollDate = e.EnrollDate,
                Status = e.Status,
                Student = e.StudentDTO == null ? null : new StudentResponse
                {
                    StudentId = e.StudentDTO.StudentId,
                    FullName = e.StudentDTO.FullName,
                    Email = e.StudentDTO.Email
                }
            }).ToList();

            // Bước D: Trả dữ liệu về Client theo format chuẩn
            return Ok(new ApiResponse<IEnumerable<EnrollmentInCourseResponse>>
            {
                Success = true,
                Message = $"Enrollments for course ID {id} retrieved successfully.",
                Data = responseData
            });
        }
    }
}
