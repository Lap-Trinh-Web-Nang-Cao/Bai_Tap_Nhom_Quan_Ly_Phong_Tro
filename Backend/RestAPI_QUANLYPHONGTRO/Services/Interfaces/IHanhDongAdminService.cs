using RestAPI_QUANLYPHONGTRO.Models;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IHanhDongAdminService
    {
        // Hàm này để các Service khác gọi ké (Ví dụ: NguoiDungService gọi để log lại việc xóa user)
        Task AddLogAsync(Guid adminId, string hanhDong, string? bang, string? recordId, string? chiTiet);

        // Lấy danh sách lịch sử (Phân trang hoặc lấy X dòng mới nhất)
        Task<IEnumerable<HanhDongAdmin>> GetLatestLogsAsync(int count);

        // Lấy lịch sử tác động lên 1 đối tượng cụ thể (Ví dụ: Xem lịch sử sửa của phòng trọ A)
        Task<IEnumerable<HanhDongAdmin>> GetLogsByRecordIdAsync(string recordId);
    }
}
