namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ITokenThongBaoService
    {
        // Đăng ký token mới (hoặc cập nhật active nếu đã có)
        Task<bool> RegisterTokenAsync(Guid userId, string token);

        // Hủy token (Khi user đăng xuất để không nhận thông báo nữa)
        Task<bool> RemoveTokenAsync(string token);
    }
}
