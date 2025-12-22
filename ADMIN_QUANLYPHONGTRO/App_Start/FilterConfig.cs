using System.Web;
using System.Web.Mvc;

namespace ADMIN_QUANLYPHONGTRO
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // Không thêm global Admin filter - Admin dùng token-based auth thay vì session
            // Các Controller có thể tùy chọn sử dụng [AdminAuthorize] nếu cần
        }
    }
}
