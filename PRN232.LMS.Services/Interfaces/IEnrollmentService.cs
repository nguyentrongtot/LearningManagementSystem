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
    public interface IEnrollmentService
    {
        Task<PagedResult<ExpandoObject>> GetEnrollmentsAsync(string? search, string? sort, int? page, int? size, string? fields, string? expand);
        Task<EnrollmentDTO?> GetEnrollmentByIdAsync(int id);
        Task<EnrollmentDTO> CreateEnrollmentAsync(EnrollmentCreateRequest createRequest);
        Task<EnrollmentDTO> UpdateEnrollmentAsync(int id, EnrollmentUpdateRequest updateRequest);
        Task<bool> DeleteEnrollmentAsync(int id);
    }
}
