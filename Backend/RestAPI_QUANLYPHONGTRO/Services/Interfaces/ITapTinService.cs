using RestAPI_QUANLYPHONGTRO.Models;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ITapTinService
    {
        // Hàm upload file và lưu thông tin vào DB
        Task<TapTin> UploadFileAsync(IFormFile file, Guid? userId);

        // Lấy thông tin file theo ID
        Task<TapTin?> GetByIdAsync(Guid id);

        // (Tuỳ chọn) Xóa file vật lý và xóa trong DB
        // Task<bool> DeleteFileAsync(Guid id);
    }
}
