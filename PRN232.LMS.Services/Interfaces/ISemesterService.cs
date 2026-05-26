using System.Threading.Tasks;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.DTOs;

namespace PRN232.LMS.Services.Interfaces
{
    public interface ISemesterService
    {
        Task<PagedResult<SemesterResponse>> GetAllSemestersAsync(string? search, string? sort, int page, int pageSize);
        Task<SemesterResponse?> GetSemesterByIdAsync(int semesterId);
        Task<SemesterResponse> CreateSemesterAsync(CreateSemesterRequest request);
        Task<SemesterResponse?> UpdateSemesterAsync(int semesterId, UpdateSemesterRequest request);
        Task<bool> DeleteSemesterAsync(int semesterId);
    }
}
