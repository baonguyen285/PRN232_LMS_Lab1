using System.Collections.Generic;
using System.Threading.Tasks;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.DTOs;

namespace PRN232.LMS.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PagedResult<CourseResponse>> GetAllCoursesAsync(string? search, string? sort, string? expand, int page, int pageSize);
        Task<CourseResponse?> GetCourseByIdAsync(int courseId, string? expand);
        Task<CourseResponse> CreateCourseAsync(CreateCourseRequest request);
        Task<CourseResponse?> UpdateCourseAsync(int courseId, UpdateCourseRequest request);
        Task<bool> DeleteCourseAsync(int courseId);
        
        // Additional teacher requirement
        Task<IEnumerable<EnrollmentResponse>?> GetEnrollmentsByCourseIdAsync(int courseId, string? expand);
    }
}

