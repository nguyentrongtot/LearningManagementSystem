using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PRN232.LMS.Repositories.DAL
{
    public class StudentRepository : IStudentRepository
    {
        private  LmsDbContext _context;
        public StudentRepository(LmsDbContext context)
        {
            _context = context;
        }
        
        public async Task<PagedResult<Student>> GetAllAsync(string? search, List<(string Field, bool IsDescending)> sortParams, int? page, int? size, bool includeEnrollments)
        {
           var students = _context.Students.AsQueryable();
            if(!string.IsNullOrWhiteSpace(search))
            {
                students = students.Where(s => s.FullName.Contains(search) || s.Email.Contains(search)|| s.DateOfBirth.ToString().Contains(search));
            }

            if (sortParams != null && sortParams.Any()) 
            {
                IOrderedQueryable<Student>? orderedStudents = null;

                foreach (var param in sortParams)
                {
                    bool isThenBy = orderedStudents != null;
                    var currentQuery = orderedStudents ?? students;

                    orderedStudents = param.Field switch
                    {
                        "fullname" => ApplyOrder(currentQuery, s => s.FullName, param.IsDescending, isThenBy),
                        "email" => ApplyOrder(currentQuery, s => s.Email, param.IsDescending, isThenBy),
                        "dateofbirth" => ApplyOrder(currentQuery, s => s.DateOfBirth, param.IsDescending, isThenBy),
                        _ => students.OrderBy(s => s.StudentId)
                    }; 
                }
                students = orderedStudents ?? students;
            }

            if (includeEnrollments)
            {
                students = students.Include(s => s.Enrollments);
            }

            // Handle pagination
            page = (page < 1 || !page.HasValue) ? 1 : page;
            size = (size < 1 || !size.HasValue) ? 10 : size;
            var totalCount = await students.CountAsync();
            var items = await students.Skip((page.Value - 1) * size.Value)
                                      .Take(size.Value)
                                      .ToListAsync();
            
            return new PagedResult<Student>(items,page.Value,size.Value,totalCount);
        }

        //Expression<Func<Student, TKey>> keySelector Ngăn lệnh thực thi ngay 
        //Nếu chỉ dùng <Func<Student, TKey>> thì sẽ thực thi ngay và không thể xây dựng truy vấn động đằng sau
        //ApplyOrder<TKey> chấp nhận nhiều kiểu dữ liệu  (ví dụ fullname -> string, email -> string , dateofbirth -> DateTime)
        public static IOrderedQueryable<Student> ApplyOrder<TKey>(IQueryable<Student> query, Expression<Func<Student, TKey>> keySelector, bool isDescending, bool isThenBy)
        {
            if (query is IOrderedQueryable<Student> orderQuery && isThenBy)
            {
                return isDescending ? orderQuery.ThenByDescending(keySelector) : orderQuery.ThenBy(keySelector);
            }
            return isDescending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }

        



        public async Task<Student?> GetByIdAsync(int id)
        {
            var query = _context.Students.AsQueryable();
            query = query.Include(s => s.Enrollments).ThenInclude(e => e.Course).ThenInclude(c => c.Semester)
                .Include(s => s.Enrollments).ThenInclude(e => e.Course).ThenInclude(c => c.Subject);
            return await query.FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<Student> CreateAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _context.Students.AnyAsync(s => s.Email == email.Trim().ToLower());
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteAsync(int id)
        {
           var studentEntity =  await _context.Students.FindAsync(id);
            if(studentEntity != null)
            {
                _context.Students.Remove(studentEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
