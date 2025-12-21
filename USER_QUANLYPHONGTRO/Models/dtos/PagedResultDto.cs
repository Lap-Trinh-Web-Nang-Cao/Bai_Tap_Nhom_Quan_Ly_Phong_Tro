using Newtonsoft.Json;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.Dtos
{
    public class PagedResultDto<T>
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}