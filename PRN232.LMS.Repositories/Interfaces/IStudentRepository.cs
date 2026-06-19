using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<PagedResult<Student>> GetAllAsync(string? search, List<(string Field, bool IsDescending)> sortParams, int? page, int? size, bool includeEnrollments);
        Task<Student?> GetByIdAsync(int id);

        Task<Student> CreateAsync(Student student);
        Task<Student> UpdateAsync(Student student);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsEmailExistAsync(string email);
    }
}
