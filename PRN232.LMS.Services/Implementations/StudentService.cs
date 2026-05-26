using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.DTOs;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IGenericRepository<Student> _studentRepository;

        public StudentService(IGenericRepository<Student> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<PagedResult<StudentResponse>> GetAllStudentsAsync(string? search, string? sort, int page, int pageSize)
        {
            var query = _studentRepository.GetQueryable();

            // 1. Searching
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(s => s.FullName.ToLower().Contains(lowerSearch) || s.Email.ToLower().Contains(lowerSearch));
            }

            // 2. Sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = QueryHelper.ApplySorting(query, sort);
            }
            else
            {
                query = query.OrderBy(s => s.StudentId); // Default sort
            }

            // 3. Paging Calculations
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var mappedItems = items.Select(s => new StudentResponse
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                DateOfBirth = s.DateOfBirth
            }).ToList();

            return new PagedResult<StudentResponse>
            {
                Items = mappedItems,
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<StudentResponse?> GetStudentByIdAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null) return null;

            return new StudentResponse
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };
        }

        public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request)
        {
            var student = new Student
            {
                FullName = request.FullName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth
            };

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();

            return new StudentResponse
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };
        }

        public async Task<StudentResponse?> UpdateStudentAsync(int studentId, UpdateStudentRequest request)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null) return null;

            student.FullName = request.FullName;
            student.Email = request.Email;
            student.DateOfBirth = request.DateOfBirth;

            _studentRepository.Update(student);
            await _studentRepository.SaveChangesAsync();

            return new StudentResponse
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };
        }

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null) return false;

            _studentRepository.Delete(student);
            await _studentRepository.SaveChangesAsync();
            return true;
        }
    }
}

