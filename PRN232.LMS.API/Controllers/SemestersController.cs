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
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemestersController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<System.Dynamic.ExpandoObject>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? fields)
        {
            var pagedResult = await _semesterService.GetSemestersAsync(search, sort, page, size, fields);
            return Ok(ApiResponseFactory.CreatePagedList(
                pagedResult.Items,
                pagedResult.Page,
                pagedResult.PageSize,
                pagedResult.TotalItems,
                pagedResult.TotalPages));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var semester = await _semesterService.GetSemesterByIdAsync(id);
            if (semester == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Message = $"Semester with ID {id} not found.",
                    Errors = new List<string> { $"No semester found with ID {id}." }
                });
            }

            var responseData = new SemesterResponse
            {
                SemesterId = semester.SemesterId,
                SemesterName = semester.SemesterName,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate
            };

            return Ok(new ApiResponse<SemesterResponse>
            {
                Success = true,
                Message = $"Request processed successfully",
                Data = responseData,
                Errors = null
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SemesterCreateRequest createRequest)
        {
            try
            {
                var created = await _semesterService.CreateSemesterAsync(RequestMapper.ToBusiness(createRequest));
                var responseData = new SemesterResponse
                {
                    SemesterId = created.SemesterId,
                    SemesterName = created.SemesterName,
                    StartDate = created.StartDate,
                    EndDate = created.EndDate
                };

                return CreatedAtAction(nameof(GetById), new { id = created.SemesterId }, new ApiResponse<SemesterResponse>
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
                    Message = "Failed to create semester.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] SemesterUpdateRequest updateRequest)
        {
            try
            {
                var updated = await _semesterService.UpdateSemesterAsync(id, RequestMapper.ToBusiness(updateRequest));
                var responseData = new SemesterResponse
                {
                    SemesterId = updated.SemesterId,
                    SemesterName = updated.SemesterName,
                    StartDate = updated.StartDate,
                    EndDate = updated.EndDate
                };

                return Ok(new ApiResponse<SemesterResponse>
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
                    Message = "Failed to update semester.",
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
                var isDeleted = await _semesterService.DeleteSemesterAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        Message = $"Semester with ID {id} not found.",
                        Errors = new List<string> { $"No semester found with ID {id}." }
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
                    Message = "Cannot delete semester.",
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting the semester.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
