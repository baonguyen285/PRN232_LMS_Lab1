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
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IGenericRepository<Enrollment> _enrollmentRepository;

        public EnrollmentService(IGenericRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<PagedResult<EnrollmentResponse>> GetAllEnrollmentsAsync(string? search, string? sort, string? expand, int page, int pageSize)
        {
            var query = _enrollmentRepository.GetQueryable();

            // 1. Expansion
            var expandList = expand?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim().ToLower()).ToList() ?? new List<string>();
            if (expandList.Contains("student"))
            {
                query = query.Include(e => e.Student);
            }
            if (expandList.Contains("course"))
            {
                query = query.Include(e => e.Course);
            }

            // 2. Searching
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(e => e.Status.ToLower().Contains(lowerSearch) || 
                                         (e.Student != null && e.Student.FullName.ToLower().Contains(lowerSearch)) ||
                                         (e.Course != null && e.Course.CourseName.ToLower().Contains(lowerSearch)));
            }

            // 3. Sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = QueryHelper.ApplySorting(query, sort);
            }
            else
            {
                query = query.OrderBy(e => e.EnrollmentId); // Default sort
            }

            // 4. Paging
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var mappedItems = items.Select(e => new EnrollmentResponse
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status,
                Student = e.Student != null ? new StudentResponse
                {
                    StudentId = e.Student.StudentId,
                    FullName = e.Student.FullName,
                    Email = e.Student.Email,
                    DateOfBirth = e.Student.DateOfBirth
                } : null,
                Course = e.Course != null ? new CourseResponse
                {
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    SemesterId = e.Course.SemesterId
                } : null
            }).ToList();

            return new PagedResult<EnrollmentResponse>
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

        public async Task<EnrollmentResponse?> GetEnrollmentByIdAsync(int enrollmentId, string? expand)
        {
            var query = _enrollmentRepository.GetQueryable().Where(e => e.EnrollmentId == enrollmentId);

            var expandList = expand?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim().ToLower()).ToList() ?? new List<string>();
            if (expandList.Contains("student"))
            {
                query = query.Include(e => e.Student);
            }
            if (expandList.Contains("course"))
            {
                query = query.Include(e => e.Course);
            }

            var enrollment = await query.FirstOrDefaultAsync();
            if (enrollment == null) return null;

            return new EnrollmentResponse
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status,
                Student = enrollment.Student != null ? new StudentResponse
                {
                    StudentId = enrollment.Student.StudentId,
                    FullName = enrollment.Student.FullName,
                    Email = enrollment.Student.Email,
                    DateOfBirth = enrollment.Student.DateOfBirth
                } : null,
                Course = enrollment.Course != null ? new CourseResponse
                {
                    CourseId = enrollment.Course.CourseId,
                    CourseName = enrollment.Course.CourseName,
                    SemesterId = enrollment.Course.SemesterId
                } : null
            };
        }

        public async Task<EnrollmentResponse> CreateEnrollmentAsync(CreateEnrollmentRequest request)
        {
            var enrollment = new Enrollment
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollDate = request.EnrollDate,
                Status = request.Status
            };

            await _enrollmentRepository.AddAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            return new EnrollmentResponse
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status
            };
        }

        public async Task<EnrollmentResponse?> UpdateEnrollmentAsync(int enrollmentId, UpdateEnrollmentRequest request)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null) return null;

            enrollment.StudentId = request.StudentId;
            enrollment.CourseId = request.CourseId;
            enrollment.EnrollDate = request.EnrollDate;
            enrollment.Status = request.Status;

            _enrollmentRepository.Update(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            return new EnrollmentResponse
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status
            };
        }

        public async Task<bool> DeleteEnrollmentAsync(int enrollmentId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null) return false;

            _enrollmentRepository.Delete(enrollment);
            await _enrollmentRepository.SaveChangesAsync();
            return true;
        }
    }
}

