using PRN232.LMS.Services.Common;

namespace PRN232.LMS.API.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public T? Data { get; set; }
        public PaginationMetadata? Pagination { get; set; }
        public object? Errors { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Request processed successfully", PaginationMetadata? pagination = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Pagination = pagination,
                Errors = null
            };
        }

        public static ApiResponse<T> Fail(string message, object? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Pagination = null,
                Errors = errors
            };
        }
    }
}

