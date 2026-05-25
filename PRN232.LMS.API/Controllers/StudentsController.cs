using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private  IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Get all students.
        /// </summary>
        /// <returns>List of students.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<StudentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery(Name = "search")] string? search, [FromQuery(Name = "sort")] string? sort, [FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? fields, [FromQuery] string? expand)
        {
            var studentsPagedResult = await _studentService.GetStudentAsync(search, sort, page, size, fields, expand);
            if (studentsPagedResult == null || !studentsPagedResult.Items.Any())
            {
                var errorResponse = new ApiErrorResponse
                {
                    Success = false,
                    Message = "No students found.",
                    Errors = new List<string> { "No students available with the criteria." }
                };
                return NotFound(errorResponse);
            }

            var apiResponses = new ApiResponse<List<System.Dynamic.ExpandoObject>>
            {
                Success = true,
                Message = "Students retrieved successfully.",
                Data = studentsPagedResult.Items, 
                Errors = null,
                Pagination = new PaginationMetadata
                {
                    Page = studentsPagedResult.Page,
                    PageSize = studentsPagedResult.PageSize,
                    TotalItems = studentsPagedResult.TotalItems,
                    TotalPages = studentsPagedResult.TotalPages
                }
            };
            return Ok(apiResponses);
        }


        /// <summary>
        /// Get student by id
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <returns>Student details.</returns>
        /// 
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StudentDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if(student == null)
            {
                var apiResponse = new ApiErrorResponse
                {
                    Success = false,
                    Message = $"Student with ID {id} not found.",
                    Errors = new List<string> { $"No student found with ID {id}." }
                };
                return NotFound(apiResponse);
            }
            var apiResponseSuccess = new ApiResponse<StudentDetailResponse>
            {
                Success = true,
                Message = $"Student with ID {id} retrieved successfully.",
                Data = new StudentDetailResponse
                {
                    StudentId = student.StudentId,
                    FullName = student.FullName,
                    DateOfBirth = student.DateOfBirth,
                    Email = student.Email,
                    Enrollments = student.EnrollmentDTOs.Select(e => new EnrollmentResponse
                    {
                        EnrollmentId = e.EnrollmentId,
                        CourseId = e.CourseId,
                        EnrollDate = e.EnrollDate,
                        Status = e.Status,
                        Course = e.CourseDTO == null ? null : new CourseResponse
                        {
                            CourseId = e.CourseDTO.CourseId,
                            CourseName = e.CourseDTO.CourseName,
                            SemesterId = e.CourseDTO.SemesterId,
                            SubjectId = e.CourseDTO.SubjectId,
                            Semester = e.CourseDTO.SemesterDTO == null ? null : new SemesterResponse
                            {
                                SemesterId = e.CourseDTO.SemesterDTO.SemesterId,
                                SemesterName = e.CourseDTO.SemesterDTO.SemesterName,
                                StartDate = e.CourseDTO.SemesterDTO.StartDate,
                                EndDate = e.CourseDTO.SemesterDTO.EndDate
                            },
                            Subject = e.CourseDTO.SubjectDTO == null ? null : new SubjectResponse
                            {
                                SubjectId = e.CourseDTO.SubjectDTO.SubjectId,
                                SubjectCode = e.CourseDTO.SubjectDTO.SubjectCode,
                                SubjectName = e.CourseDTO.SubjectDTO.SubjectName,
                                Credit = e.CourseDTO.SubjectDTO.Credit
                            }
                        }
                    }).ToList()
                },
                Errors = null
            };
            return Ok(apiResponseSuccess);
        }

        /// <summary>
        /// Create a new student.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] StudentCreateRequest createRequest)
        {
            try
            {
                var students = await _studentService.CreateStudentAsync(createRequest);
                
                var responseData = new StudentResponse
                {
                    StudentId = students.StudentId,
                    FullName = students.FullName,
                    DateOfBirth = students.DateOfBirth,
                    Email = students.Email
                };

                var successResponse = new ApiResponse<StudentResponse>
                {
                    Success = true,
                    Message = "Student created successfully.",
                    Data = responseData,
                    Errors = (List<string>?)null
                };

                return CreatedAtAction(nameof(GetById), new { id = students.StudentId }, successResponse);
                //return Ok(successResponse);
            }
            catch(Exception ex)
            {
                var apiResponse = new ApiErrorResponse
                {
                    Success = false,
                    Message = "Failed to create student.",
                    Errors = new List<string> { ex.Message } 
                };
                return BadRequest(apiResponse);
            }
        }

        /// <summary>
        /// Update student by id
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] StudentUpdateRequest updateRequest)
        {
            try
            {
                var student = await _studentService.UpdateStudentAsync(id, updateRequest);

                var responseData = new StudentResponse
                {
                    StudentId = student.StudentId,
                    FullName = student.FullName,
                    DateOfBirth = student.DateOfBirth,
                    Email = student.Email
                };

                var successResponse = new ApiResponse<StudentResponse>
                {
                    Success = true,
                    Message = $"Student with ID {id} updated successfully.",
                    Data = responseData,
                    Errors = null
                };
                return Ok(successResponse);
            }
            catch(Exception ex)
            {
                var apiResponse = new ApiErrorResponse
                {
                    Success = false,
                    Message = "Failed to update student.",
                    Errors = new List<string> { ex.Message }
                };
                return BadRequest(apiResponse);
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
                var isDeleted = await _studentService.DeleteStudentAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        Message = $"Student with ID {id} not found.",
                        Errors = new List<string> { $"No student found with ID {id}." }
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = $"Student with ID {id} deleted successfully.",
                    Data = null,
                    Errors = null
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Message = "Cannot delete student.",
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting the student.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
