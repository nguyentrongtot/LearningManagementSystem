using System.Collections.Generic;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.EntityModel;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface ISemesterRepository
    {
        Task<PagedResult<Semester>> GetAllAsync(string? search, List<(string Field, bool IsDescending)> sortParams, int? page, int? size);
        Task<Semester?> GetByIdAsync(int id);
        Task<Semester> CreateAsync(Semester semester);
        Task<Semester> UpdateAsync(Semester semester);
        Task<bool> DeleteAsync(int id);
    }
}
