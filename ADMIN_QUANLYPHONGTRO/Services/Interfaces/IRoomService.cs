using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.ViewModels;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IRoomService
    {
        // Lấy danh sách phòng chờ duyệt (với phân trang)
        Task<PagedResult<RoomPendingItemViewModel>> GetPendingRoomsAsync(int pageIndex, int pageSize, string keyword = "");
        
        // Lấy danh sách tất cả phòng (với phân trang)
        Task<PagedResult<RoomPendingItemViewModel>> GetAllRoomsAsync(int pageIndex, int pageSize, string keyword = "");
        
        // Lấy chi tiết phòng để duyệt
        Task<RoomPendingItemViewModel> GetRoomDetailAsync(string roomId);
        
        // Duyệt phòng
        Task<bool> ApproveRoomAsync(string roomId);
        
        // Từ chối phòng
        Task<bool> RejectRoomAsync(string roomId, string reason);
        
        // Khóa/Mở khóa phòng
        Task<bool> ToggleLockRoomAsync(string roomId, bool isLocked = true);

        // Lấy thống kê phòng
        Task<RoomStatsViewModel> GetRoomStatsAsync();
    }

    public class RoomStatsViewModel
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Locked { get; set; }
    }
}
