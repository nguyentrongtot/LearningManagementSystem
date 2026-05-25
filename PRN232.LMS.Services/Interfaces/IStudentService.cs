using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Interfaces
{
    public interface IStudentService
    {
        //Task<PagedResult<StudentDTO>> GetStudentAsync(string? searchTerm, string? sortTerm, int? page, int? size);
        Task<PagedResult<ExpandoObject>> GetStudentAsync(string? searchTerm, string? sort, int? page, int? size, string? fields,string? expand);
        Task<StudentDTO?> GetStudentByIdAsync(int id);
        Task<StudentDTO> CreateStudentAsync(StudentCreateRequest createRequest);
        Task<StudentDTO> UpdateStudentAsync(int id,StudentUpdateRequest updateRequest);    
        Task<bool> DeleteStudentAsync(int id);
    }
}
