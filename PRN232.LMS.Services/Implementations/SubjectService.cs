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
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly ICourseRepository _courseRepository;

        public SubjectService(ISubjectRepository subjectRepository, ICourseRepository courseRepository)
        {
            _subjectRepository = subjectRepository;
            _courseRepository = courseRepository;
        }

        public async Task<PagedResult<ExpandoObject>> GetSubjectsAsync(string? search, string? sort, int? page = 1, int? size = 10, string? fields = null)
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

            var subjects = await _subjectRepository.GetAllAsync(search, sortParams, page, size);
            
            var dtos = subjects.Items.Select(s => new SubjectDTO
            {
                SubjectId = s.SubjectId,
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
                Credit = s.Credit
            }).ToList();

            var shaped = dtos.Select(dto => DataShaper.ShapeData(dto, fields)).ToList();

            return new PagedResult<ExpandoObject>(shaped, subjects.Page, subjects.PageSize, subjects.TotalItems);
        }

        public async Task<SubjectDTO?> GetSubjectByIdAsync(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null) return null;

            return new SubjectDTO
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credit = subject.Credit
            };
        }

        public async Task<SubjectDTO> CreateSubjectAsync(SubjectCreateRequest createRequest)
        {
            if (await _subjectRepository.IsCodeExistAsync(createRequest.SubjectCode))
                throw new ArgumentException($"Subject code {createRequest.SubjectCode} already exists.");

            var entity = new Subject
            {
                SubjectCode = createRequest.SubjectCode.Trim(),
                SubjectName = createRequest.SubjectName,
                Credit = createRequest.Credit
            };

            var created = await _subjectRepository.CreateAsync(entity);
            return new SubjectDTO
            {
                SubjectId = created.SubjectId,
                SubjectCode = created.SubjectCode,
                SubjectName = created.SubjectName,
                Credit = created.Credit
            };
        }

        public async Task<SubjectDTO> UpdateSubjectAsync(int id, SubjectUpdateRequest updateRequest)
        {
            var entity = await _subjectRepository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Subject ID {id} not found.");

            if (entity.SubjectCode != updateRequest.SubjectCode.Trim() && await _subjectRepository.IsCodeExistAsync(updateRequest.SubjectCode))
                throw new ArgumentException($"Subject code {updateRequest.SubjectCode} already exists.");

            entity.SubjectCode = updateRequest.SubjectCode.Trim();
            entity.SubjectName = updateRequest.SubjectName;
            entity.Credit = updateRequest.Credit;

            await _subjectRepository.UpdateAsync(entity);

            return new SubjectDTO
            {
                SubjectId = entity.SubjectId,
                SubjectCode = entity.SubjectCode,
                SubjectName = entity.SubjectName,
                Credit = entity.Credit
            };
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            if (await _courseRepository.HasCoursesBySubjectAsync(id))
            {
                throw new InvalidOperationException($"Không thể xóa môn học với ID {id} vì đã có khóa học thuộc môn học này.");
            }
            return await _subjectRepository.DeleteAsync(id);
        }
    }
}
