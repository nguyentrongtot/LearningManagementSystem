using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.EntityModel;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Implementations
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly ICourseRepository _courseRepository;

        public SemesterService(ISemesterRepository semesterRepository, ICourseRepository courseRepository)
        {
            _semesterRepository = semesterRepository;
            _courseRepository = courseRepository;
        }

        public async Task<PagedResult<ExpandoObject>> GetSemestersAsync(string? search, string? sort, int? page = 1, int? size = 10, string? fields = null)
        {
            var sortParams = new List<(string Field, bool IsDescending)>();
            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortExpression = sort.Split(',');
                foreach (var expression in sortExpression)
                {
                    var trimmedExpression = expression.Trim().ToLower();
                    var isDescending = trimmedExpression.StartsWith("-");
                    var propertyName = isDescending ? trimmedExpression.Substring(1) : trimmedExpression;
                    sortParams.Add((propertyName, isDescending));
                }
            }

            var semesters = await _semesterRepository.GetAllAsync(search, sortParams, page, size);
            
            var dtos = semesters.Items.Select(s => new SemesterDTO
            {
                SemesterId = s.SemesterId,
                SemesterName = s.SemesterName,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            }).ToList();

            var shaped = dtos.Select(dto => DataShaper.ShapeData(dto, fields)).ToList();

            return new PagedResult<ExpandoObject>(shaped, semesters.Page, semesters.PageSize, semesters.TotalItems);
        }

        public async Task<SemesterDTO?> GetSemesterByIdAsync(int id)
        {
            var semester = await _semesterRepository.GetByIdAsync(id);
            if (semester == null) return null;

            return new SemesterDTO
            {
                SemesterId = semester.SemesterId,
                SemesterName = semester.SemesterName,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate
            };
        }

        public async Task<SemesterDTO> CreateSemesterAsync(SemesterCreateRequest createRequest)
        {
            if (createRequest.StartDate >= createRequest.EndDate)
                throw new ArgumentException("StartDate must be earlier than EndDate.");

            var entity = new Semester
            {
                SemesterName = createRequest.SemesterName,
                StartDate = createRequest.StartDate,
                EndDate = createRequest.EndDate
            };

            var created = await _semesterRepository.CreateAsync(entity);
            return new SemesterDTO
            {
                SemesterId = created.SemesterId,
                SemesterName = created.SemesterName,
                StartDate = created.StartDate,
                EndDate = created.EndDate
            };
        }

        public async Task<SemesterDTO> UpdateSemesterAsync(int id, SemesterUpdateRequest updateRequest)
        {
            var entity = await _semesterRepository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Semester ID {id} not found.");

            if (updateRequest.StartDate >= updateRequest.EndDate)
                throw new ArgumentException("StartDate must be earlier than EndDate.");

            entity.SemesterName = updateRequest.SemesterName;
            entity.StartDate = updateRequest.StartDate;
            entity.EndDate = updateRequest.EndDate;

            await _semesterRepository.UpdateAsync(entity);

            return new SemesterDTO
            {
                SemesterId = entity.SemesterId,
                SemesterName = entity.SemesterName,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate
            };
        }

        public async Task<bool> DeleteSemesterAsync(int id)
        {
            if (await _courseRepository.HasCoursesBySemesterAsync(id))
            {
                throw new InvalidOperationException($"Không thể xóa học kỳ với ID {id} vì đã có khóa học thuộc học kỳ này.");
            }
            return await _semesterRepository.DeleteAsync(id);
        }
    }
}
