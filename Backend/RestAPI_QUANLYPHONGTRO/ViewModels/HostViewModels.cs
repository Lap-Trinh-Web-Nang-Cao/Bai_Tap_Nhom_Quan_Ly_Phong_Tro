using System;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    /// <summary>
    /// DTO cho danh sách chủ trọ chờ duyệt
    /// </summary>
    public class HostPendingDto
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string Avatar { get; set; }
        
        // Thông tin pháp lý
        public string SoCCCD { get; set; }
        public string LoaiGiayTo { get; set; }
        
        // Giấy tờ đính kèm
        public int SoTapTinDinhKem { get; set; }
        public bool DaTaiGiayTo { get; set; }
        
        // Thời gian
        public DateTime NgayDangKy { get; set; }
        
        // Trạng thái
        public string TrangThaiXacThuc { get; set; }
    }

    /// <summary>
    /// DTO chi tiết chủ trọ để duyệt (Modal)
    /// </summary>
    public class HostApprovalDto
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public DateTime NgaySinh { get; set; }
        public string QueQuan { get; set; }
        public string SoCCCD { get; set; }
        public string Avatar { get; set; }
        
        // Ảnh giấy tờ
        public string CCCDMatTruocUrl { get; set; }
        public string CCCDMatSauUrl { get; set; }
        public string GiayPhepKinhDoanhUrl { get; set; }
        
        // Trạng thái
        public string TrangThaiXacThuc { get; set; }
    }

    /// <summary>
    /// Request từ chối chủ trọ
    /// </summary>
    public class RejectHostRequest
    {
        public string Reason { get; set; }
    }
}
