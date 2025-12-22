namespace ADMIN_QUANLYPHONGTRO.Models.Common
{
    public class PagingRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Keyword { get; set; }  // dùng cho tìm kiếm
    }
}
