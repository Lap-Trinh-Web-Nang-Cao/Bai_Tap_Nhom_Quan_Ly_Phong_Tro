using System;

namespace USER_QUANLYPHONGTRO.Services
{
    /// <summary>
    /// Response từ API đăng nhập
    /// </summary>
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int VaiTroId { get; set; }
    }

    /// <summary>
    /// Thông tin giải mã từ JWT Token
    /// </summary>
    public class UserTokenInfo
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int VaiTroId { get; set; }
    }

    /// <summary>
    /// Thông tin lưu trong Session
    /// </summary>
    public class UserSessionInfo
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int VaiTroId { get; set; }
        public string RoleName { get; set; }
        public string AuthToken { get; set; }

        public bool IsAdmin => VaiTroId == 1;
        public bool IsChuTro => VaiTroId == 2;
        public bool IsNguoiThue => VaiTroId == 3;

        // Backward compatibility properties
        public string UserEmail
        {
            get { return Email; }
            set { Email = value; }
        }

        public string UserRole
        {
            get { return RoleName; }
            set { RoleName = value; }
        }
    }
}
