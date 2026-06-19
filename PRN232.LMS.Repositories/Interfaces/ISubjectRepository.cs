using System.Collections.Generic;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        Task<PagedResult<Subject>> GetAllAsync(string? search, List<(string Field, bool IsDescending)> sortParams, int? page, int? size);
        Task<Subject?> GetByIdAsync(int id);
        Task<Subject> CreateAsync(Subject subject);
        Task<Subject> UpdateAsync(Subject subject);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsCodeExistAsync(string code);
    }
}
