using System.Collections.Generic;

namespace PRN232.LMS.Services.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public PaginationMetadata Pagination { get; set; } = null!;
    }
}

