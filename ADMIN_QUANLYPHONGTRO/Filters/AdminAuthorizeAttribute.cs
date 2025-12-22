using System;
using System.Web;
using System.Web.Mvc;
using ADMIN_QUANLYPHONGTRO.Services;

namespace ADMIN_QUANLYPHONGTRO.Filters
{
    /// <summary>
    /// Custom Authorization Filter cho Admin
    /// Chỉ cho phép Admin (VaiTroId = 1) truy cập
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            // Kiểm tra xem Admin đã đăng nhập chưa
            var authService = new AdminAuthService();
            
            System.Diagnostics.Debug.WriteLine($"🔵 AdminAuthorizeAttribute.AuthorizeCore - Checking admin session");
            System.Diagnostics.Debug.WriteLine($"   IsAdminLoggedIn: {authService.IsAdminLoggedIn()}");
            
            if (!authService.IsAdminLoggedIn())
            {
                System.Diagnostics.Debug.WriteLine($"❌ AdminAuthorizeAttribute: Admin NOT logged in");
                return false;
            }

            // Kiểm tra xem có phải Admin không
            var adminSession = authService.GetCurrentAdminSession();
            System.Diagnostics.Debug.WriteLine($"   AdminSession: {(adminSession == null ? "NULL" : $"Email={adminSession.UserEmail}, VaiTroId={adminSession.VaiTroId}")}");
            
            return adminSession != null && adminSession.VaiTroId == 1; // 1 = Admin
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException(nameof(filterContext));

            // Bỏ qua nếu có [AllowAnonymousAttribute]
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length > 0 ||
                filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length > 0)
            {
                return;
            }

            if (!AuthorizeCore(filterContext.HttpContext))
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Auth/Login");
        }
    }

    /// <summary>
    /// Attribute cho phép bỏ qua xác thực
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {
    }
}
