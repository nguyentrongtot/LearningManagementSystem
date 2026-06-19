using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.API.Models.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<System.Dynamic.ExpandoObject>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? fields, [FromQuery] string? expand)
        {
            var pagedResult = await _enrollmentService.GetEnrollmentsAsync(search, sort, page, size, fields, expand);
            return Ok(ApiResponseFactory.CreatePagedList(
                pagedResult.Items,
                pagedResult.Page,
                pagedResult.PageSize,
                pagedResult.TotalItems,
                pagedResult.TotalPages));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (enrollment == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Message = $"Enrollment with ID {id} not found.",
                    Errors = new List<string> { $"No enrollment found with ID {id}." }
                });
            }

            var responseData = new EnrollmentResponse
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status,
                StudentResponse = enrollment.StudentDTO == null ? null : new StudentResponse
                {
                    StudentId = enrollment.StudentDTO.StudentId,
                    FullName = enrollment.StudentDTO.FullName,
                    Email = enrollment.StudentDTO.Email,
                    DateOfBirth = enrollment.StudentDTO.DateOfBirth
                },
                CourseResponse = enrollment.CourseDTO == null ? null : new CourseResponse
                {
                    CourseId = enrollment.CourseDTO.CourseId,
                    CourseName = enrollment.CourseDTO.CourseName,
                    SemesterId = enrollment.CourseDTO.SemesterId,
                    SubjectId = enrollment.CourseDTO.SubjectId,
                    Semester = enrollment.CourseDTO.SemesterDTO == null ? null : new SemesterResponse
                    {
                        SemesterId = enrollment.CourseDTO.SemesterDTO.SemesterId,
                        SemesterName = enrollment.CourseDTO.SemesterDTO.SemesterName,
                        StartDate = enrollment.CourseDTO.SemesterDTO.StartDate,
                        EndDate = enrollment.CourseDTO.SemesterDTO.EndDate
                    },
                    Subject = enrollment.CourseDTO.SubjectDTO == null ? null : new SubjectResponse
                    {
                        SubjectId = enrollment.CourseDTO.SubjectDTO.SubjectId,
                        SubjectCode = enrollment.CourseDTO.SubjectDTO.SubjectCode,
                        SubjectName = enrollment.CourseDTO.SubjectDTO.SubjectName,
                        Credit = enrollment.CourseDTO.SubjectDTO.Credit
                    }
                }
            };

            return Ok(new ApiResponse<EnrollmentResponse>
            {
                Success = true,
                Message = $"Request processed successfully",
                Data = responseData,
                Errors = null
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] EnrollmentCreateRequest createRequest)
        {
            try
            {
                var created = await _enrollmentService.CreateEnrollmentAsync(RequestMapper.ToBusiness(createRequest));
                var responseData = new EnrollmentResponse
                {
                    EnrollmentId = created.EnrollmentId,
                    CourseId = created.CourseId,
                    StudentId = created.StudentId,
                    EnrollDate = created.EnrollDate,
                    Status = created.Status,
                    StudentResponse = new StudentResponse
                    {
                        StudentId = created.StudentDTO.StudentId,
                        FullName = created.StudentDTO.FullName,
                        Email = created.StudentDTO.Email,
                        DateOfBirth = created.StudentDTO.DateOfBirth
                    },
                    CourseResponse = new CourseResponse
                    {
                        CourseId = created.CourseDTO.CourseId,
                        CourseName = created.CourseDTO.CourseName,
                        SemesterId = created.CourseDTO.SemesterId,
                        SubjectId = created.CourseDTO.SubjectId,
                        Semester = created.CourseDTO.SemesterDTO == null ? null : new SemesterResponse
                        {
                            SemesterId = created.CourseDTO.SemesterDTO.SemesterId,
                            SemesterName = created.CourseDTO.SemesterDTO.SemesterName,
                            StartDate = created.CourseDTO.SemesterDTO.StartDate,
                            EndDate = created.CourseDTO.SemesterDTO.EndDate
                        },
                        Subject = created.CourseDTO.SubjectDTO == null ? null : new SubjectResponse
                        {
                            SubjectId = created.CourseDTO.SubjectDTO.SubjectId,
                            SubjectCode = created.CourseDTO.SubjectDTO.SubjectCode,
                            SubjectName = created.CourseDTO.SubjectDTO.SubjectName,
                            Credit = created.CourseDTO.SubjectDTO.Credit
                        }
                    }

                };

                return CreatedAtAction(nameof(GetById), new { id = created.EnrollmentId }, new ApiResponse<EnrollmentResponse>
                {
                    Success = true,
                    Message = "Request processed successfully",
                    Data = responseData,
                    Errors = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Message = "Failed to create enrollment.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] EnrollmentUpdateRequest updateRequest)
        {
            try
            {
                var updated = await _enrollmentService.UpdateEnrollmentAsync(id, RequestMapper.ToBusiness(updateRequest));
                var responseData = new EnrollmentResponse
                {
                    EnrollmentId = updated.EnrollmentId,
                    CourseId = updated.CourseId,
                    EnrollDate = updated.EnrollDate,
                    Status = updated.Status
                };

                return Ok(new ApiResponse<EnrollmentResponse>
                {
                    Success = true,
                    Message = $"Request processed successfully",
                    Data = responseData,
                    Errors = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Message = "Failed to update enrollment.",
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
                var isDeleted = await _enrollmentService.DeleteEnrollmentAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        Message = $"Enrollment with ID {id} not found.",
                        Errors = new List<string> { $"No enrollment found with ID {id}." }
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = $"Request processed successfully",
                    Data = null,
                    Errors = null
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Message = "Cannot delete enrollment.",
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting the enrollment.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
