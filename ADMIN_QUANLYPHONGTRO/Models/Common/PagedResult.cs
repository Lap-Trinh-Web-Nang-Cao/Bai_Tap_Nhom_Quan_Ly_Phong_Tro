using System.Collections.Generic;

namespace ADMIN_QUANLYPHONGTRO.Models.Common
{
    public class PagedResult<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public List<T> Items { get; set; }
    }
}
