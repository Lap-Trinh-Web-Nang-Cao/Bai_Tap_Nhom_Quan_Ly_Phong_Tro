using System.Web.Mvc;
using ADMIN_QUANLYPHONGTRO.Services;
using ADMIN_QUANLYPHONGTRO.Filters;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Base Controller cho Admin - có xác thực
    /// Các Controller khác kế thừa từ đây
    /// </summary>
    [AdminAuthorize]
    public class AdminControllerBase : Controller
    {
        protected AdminAuthService AuthService { get; set; }
        protected AdminSessionInfo CurrentAdmin { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            AuthService = new AdminAuthService();
            CurrentAdmin = AuthService.GetCurrentAdminSession();

            ViewBag.CurrentAdmin = CurrentAdmin;
            ViewBag.IsAdmin = CurrentAdmin?.IsAdmin ?? false;
        }
    }
}
