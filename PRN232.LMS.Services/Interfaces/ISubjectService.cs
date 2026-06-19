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
    public interface ISubjectService
    {
        Task<PagedResult<ExpandoObject>> GetSubjectsAsync(string? search, string? sort, int? page, int? size, string? fields);
        Task<SubjectDTO?> GetSubjectByIdAsync(int id);
        Task<SubjectDTO> CreateSubjectAsync(SubjectCreateRequest createRequest);
        Task<SubjectDTO> UpdateSubjectAsync(int id, SubjectUpdateRequest updateRequest);
        Task<bool> DeleteSubjectAsync(int id);
    }
}
