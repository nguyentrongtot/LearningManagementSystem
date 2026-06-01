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
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<System.Dynamic.ExpandoObject>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? fields)
        {
            var pagedResult = await _subjectService.GetSubjectsAsync(search, sort, page, size, fields);
            if (pagedResult == null || pagedResult.Items.Count == 0)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Message = "No subjects found.",
                    Errors = new List<string> { "No subjects available with the given criteria." }
                });
            }

            return Ok(new ApiResponse<List<System.Dynamic.ExpandoObject>>
            {
                Success = true,
                Message = "Request processed successfully",
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
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Message = $"Subject with ID {id} not found.",
                    Errors = new List<string> { $"No subject found with ID {id}." }
                });
            }

            var responseData = new SubjectResponse
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credit = subject.Credit
            };

            return Ok(new ApiResponse<SubjectResponse>
            {
                Success = true,
                Message = $"Request processed successfully",
                Data = responseData,
                Errors = null
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SubjectCreateRequest createRequest)
        {
            try
            {
                var created = await _subjectService.CreateSubjectAsync(createRequest);
                var responseData = new SubjectResponse
                {
                    SubjectId = created.SubjectId,
                    SubjectCode = created.SubjectCode,
                    SubjectName = created.SubjectName,
                    Credit = created.Credit
                };

                return CreatedAtAction(nameof(GetById), new { id = created.SubjectId }, new ApiResponse<SubjectResponse>
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
                    Message = "Failed to create subject.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] SubjectUpdateRequest updateRequest)
        {
            try
            {
                var updated = await _subjectService.UpdateSubjectAsync(id, updateRequest);
                var responseData = new SubjectResponse
                {
                    SubjectId = updated.SubjectId,
                    SubjectCode = updated.SubjectCode,
                    SubjectName = updated.SubjectName,
                    Credit = updated.Credit
                };

                return Ok(new ApiResponse<SubjectResponse>
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
                    Message = "Failed to update subject.",
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
                var isDeleted = await _subjectService.DeleteSubjectAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        Message = $"Subject with ID {id} not found.",
                        Errors = new List<string> { $"No subject found with ID {id}." }
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
                    Message = "Cannot delete subject.",
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting the subject.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
