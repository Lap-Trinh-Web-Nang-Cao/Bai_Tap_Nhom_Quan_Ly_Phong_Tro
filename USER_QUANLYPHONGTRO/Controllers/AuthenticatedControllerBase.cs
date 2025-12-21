using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Services;
using USER_QUANLYPHONGTRO.Filters;

namespace USER_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Base Controller với xác thực
    /// Các Controller khác kế thừa từ đây sẽ tự động kiểm tra đăng nhập
    /// </summary>
    [CustomAuthorizeAttribute]
    public class AuthenticatedControllerBase : Controller
    {
        protected AuthService AuthService { get; set; }
        protected UserSessionInfo CurrentUser { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Khởi tạo AuthService
            AuthService = new AuthService();

            // Lấy thông tin người dùng hiện tại
            CurrentUser = AuthService.GetCurrentUserSession();

            // Truyền thông tin vào View
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.IsAdmin = CurrentUser?.IsAdmin ?? false;
            ViewBag.IsChuTro = CurrentUser?.IsChuTro ?? false;
            ViewBag.IsNguoiThue = CurrentUser?.IsNguoiThue ?? false;
        }
    }
}
