using System.Threading.Tasks;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.DTOs;

namespace PRN232.LMS.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<PagedResult<SubjectResponse>> GetAllSubjectsAsync(string? search, string? sort, int page, int pageSize);
        Task<SubjectResponse?> GetSubjectByIdAsync(int subjectId);
        Task<SubjectResponse> CreateSubjectAsync(CreateSubjectRequest request);
        Task<SubjectResponse?> UpdateSubjectAsync(int subjectId, UpdateSubjectRequest request);
        Task<bool> DeleteSubjectAsync(int subjectId);
    }
}
