using System;
using System.Web;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Filters
{
    /// <summary>
    /// Custom Authorization Filter để yêu cầu đăng nhập
    /// Sử dụng: [CustomAuthorizeAttribute] trên Controller/Action
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly int[] _allowedRoles;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="roles">Danh sách VaiTroId được phép (1=Admin, 2=ChuTro, 3=NguoiThue)</param>
        public CustomAuthorizeAttribute(params int[] roles)
        {
            _allowedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            // Kiểm tra người dùng đã đăng nhập chưa
            var authService = new AuthService();
            if (!authService.IsUserLoggedIn())
                return false;

            // Nếu không chỉ định role cụ thể, chỉ cần đăng nhập là được
            if (_allowedRoles == null || _allowedRoles.Length == 0)
                return true;

            // Kiểm tra xem người dùng có role được phép không
            var userSession = authService.GetCurrentUserSession();
            if (userSession == null)
                return false;

            foreach (var roleId in _allowedRoles)
            {
                if (userSession.VaiTroId == roleId)
                    return true;
            }

            return false;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException(nameof(filterContext));

            // Kiểm tra xem Action/Controller có [AllowAnonymousAttribute] không
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length > 0 ||
                filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length > 0)
            {
                return;
            }

            // Thực hiện xác thực
            if (!AuthorizeCore(filterContext.HttpContext))
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Auth/Login?returnUrl=" +
                System.Uri.EscapeDataString(filterContext.HttpContext.Request.RawUrl));
        }
    }

    /// <summary>
    /// Attribute để bỏ qua xác thực (dùng kết hợp với [CustomAuthorizeAttribute])
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {
    }
}