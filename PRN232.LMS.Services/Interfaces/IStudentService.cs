using System.Threading.Tasks;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.DTOs;

namespace PRN232.LMS.Services.Interfaces
{
    public interface IStudentService
    {
        Task<PagedResult<StudentResponse>> GetAllStudentsAsync(string? search, string? sort, int page, int pageSize);
        Task<StudentResponse?> GetStudentByIdAsync(int studentId);
        Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request);
        Task<StudentResponse?> UpdateStudentAsync(int studentId, UpdateStudentRequest request);
        Task<bool> DeleteStudentAsync(int studentId);
    }
}

