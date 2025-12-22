using System;
using System.Threading.Tasks;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Services.Interfaces
{
    /// <summary>
    /// Interface xác thực - Xử lý đăng nhập, token, session
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Đăng nhập với Email & Password
        /// </summary>
        Task<LoginResponse> LoginAsync(string email, string password);

        /// <summary>
        /// Lấy thông tin người dùng từ Token
        /// </summary>
        UserTokenInfo GetUserInfoFromToken(string token);

        /// <summary>
        /// Lưu Token & Thông tin vào Session
        /// </summary>
        void SaveUserSessionFromToken(string token, UserTokenInfo userInfo);

        /// <summary>
        /// Kiểm tra người dùng đã đăng nhập chưa
        /// </summary>
        bool IsUserLoggedIn();

        /// <summary>
        /// Lấy Token từ Session
        /// </summary>
        string GetAuthToken();

        /// <summary>
        /// Lấy thông tin người dùng từ Session
        /// </summary>
        UserSessionInfo GetCurrentUserSession();

        /// <summary>
        /// Đăng xuất - Xóa Session
        /// </summary>
        void Logout();
    }
}
