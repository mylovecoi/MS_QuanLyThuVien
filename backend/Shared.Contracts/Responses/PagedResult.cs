using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Contracts.Responses
{
    /// <summary>
    /// Envelope cho các endpoint trả về danh sách có phân trang.
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = PagedResultDefaults.PageSize;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }

        public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int page, int pageSize)
        {
            var effectivePageSize = pageSize < 1 ? PagedResultDefaults.PageSize : pageSize;
            var totalPages = totalCount <= 0
                ? 0
                : (int)Math.Ceiling(totalCount / (double)effectivePageSize);

            return new PagedResult<T>
            {
                Items = items.ToList(),
                Page = page < 1 ? 1 : page,
                PageSize = effectivePageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPrevious = page > 1,
                HasNext = page < totalPages
            };
        }
    }

    public static class PagedResultDefaults
    {
        public const int PageSize = 20;

        /// <summary>
        /// Trần cứng cho pageSize. Phải được clamp ở controller, nếu không
        /// client có thể yêu cầu pageSize khổng lồ và làm cạn bộ nhớ.
        /// </summary>
        public const int MaxPageSize = 200;
    }
}