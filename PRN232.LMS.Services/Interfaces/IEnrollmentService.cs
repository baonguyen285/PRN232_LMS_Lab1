using System.Threading.Tasks;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.DTOs;

namespace PRN232.LMS.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<PagedResult<EnrollmentResponse>> GetAllEnrollmentsAsync(string? search, string? sort, string? expand, int page, int pageSize);
        Task<EnrollmentResponse?> GetEnrollmentByIdAsync(int enrollmentId, string? expand);
        Task<EnrollmentResponse> CreateEnrollmentAsync(CreateEnrollmentRequest request);
        Task<EnrollmentResponse?> UpdateEnrollmentAsync(int enrollmentId, UpdateEnrollmentRequest request);
        Task<bool> DeleteEnrollmentAsync(int enrollmentId);
    }
}

