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
    public interface ISemesterService
    {
        Task<PagedResult<ExpandoObject>> GetSemestersAsync(string? search, string? sort, int? page, int? size, string? fields);
        Task<SemesterDTO?> GetSemesterByIdAsync(int id);
        Task<SemesterDTO> CreateSemesterAsync(SemesterCreateRequest createRequest);
        Task<SemesterDTO> UpdateSemesterAsync(int id, SemesterUpdateRequest updateRequest);
        Task<bool> DeleteSemesterAsync(int id);
    }
}
