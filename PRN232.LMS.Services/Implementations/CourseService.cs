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
    public class CourseService : ICourseService
    {
        private readonly IGenericRepository<Course> _courseRepository;
        private readonly IGenericRepository<Enrollment> _enrollmentRepository;

        public CourseService(
            IGenericRepository<Course> courseRepository,
            IGenericRepository<Enrollment> enrollmentRepository)
        {
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<PagedResult<CourseResponse>> GetAllCoursesAsync(string? search, string? sort, string? expand, int page, int pageSize)
        {
            var query = _courseRepository.GetQueryable();

            // 1. Expansion
            var expandList = expand?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim().ToLower()).ToList() ?? new List<string>();
            if (expandList.Contains("semester"))
            {
                query = query.Include(c => c.Semester);
            }

            // 2. Searching
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(c => c.CourseName.ToLower().Contains(lowerSearch));
            }

            // 3. Sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = QueryHelper.ApplySorting(query, sort);
            }
            else
            {
                query = query.OrderBy(c => c.CourseId); // Default sort
            }

            // 4. Paging
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var mappedItems = items.Select(c => new CourseResponse
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                SemesterId = c.SemesterId,
                Semester = c.Semester != null ? new SemesterResponse
                {
                    SemesterId = c.Semester.SemesterId,
                    SemesterName = c.Semester.SemesterName,
                    StartDate = c.Semester.StartDate,
                    EndDate = c.Semester.EndDate
                } : null
            }).ToList();

            return new PagedResult<CourseResponse>
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

        public async Task<CourseResponse?> GetCourseByIdAsync(int courseId, string? expand)
        {
            var query = _courseRepository.GetQueryable().Where(c => c.CourseId == courseId);

            var expandList = expand?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim().ToLower()).ToList() ?? new List<string>();
            if (expandList.Contains("semester"))
            {
                query = query.Include(c => c.Semester);
            }

            var course = await query.FirstOrDefaultAsync();
            if (course == null) return null;

            return new CourseResponse
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId,
                Semester = course.Semester != null ? new SemesterResponse
                {
                    SemesterId = course.Semester.SemesterId,
                    SemesterName = course.Semester.SemesterName,
                    StartDate = course.Semester.StartDate,
                    EndDate = course.Semester.EndDate
                } : null
            };
        }

        public async Task<CourseResponse> CreateCourseAsync(CreateCourseRequest request)
        {
            var course = new Course
            {
                CourseName = request.CourseName,
                SemesterId = request.SemesterId
            };

            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();

            return new CourseResponse
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId
            };
        }

        public async Task<CourseResponse?> UpdateCourseAsync(int courseId, UpdateCourseRequest request)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null) return null;

            course.CourseName = request.CourseName;
            course.SemesterId = request.SemesterId;

            _courseRepository.Update(course);
            await _courseRepository.SaveChangesAsync();

            return new CourseResponse
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId
            };
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null) return false;

            _courseRepository.Delete(course);
            await _courseRepository.SaveChangesAsync();
            return true;
        }

        // Additional teacher requirement
        public async Task<IEnumerable<EnrollmentResponse>?> GetEnrollmentsByCourseIdAsync(int courseId, string? expand)
        {
            // 1. Verify Course Exists
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null) return null;

            // 2. Query Enrollments
            var query = _enrollmentRepository.GetQueryable().Where(e => e.CourseId == courseId);

            // 3. Optional Expansion (e.g., student)
            var expandList = expand?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim().ToLower()).ToList() ?? new List<string>();
            if (expandList.Contains("student"))
            {
                query = query.Include(e => e.Student);
            }

            var enrollments = await query.ToListAsync();

            return enrollments.Select(e => new EnrollmentResponse
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
                } : null
            });
        }
    }
}

