using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    /// <summary>
    /// DTO cho Yêu cầu hỗ trợ - mapping từ API Backend
    /// </summary>
    public class YeuCauHoTroDto
    {
        public Guid HoTroId { get; set; }
        
        /// <summary>
        /// ID phòng xảy ra sự cố (có thể null nếu không liên quan đến phòng)
        /// </summary>
        public Guid? PhongId { get; set; }
        
        /// <summary>
        /// ID loại hỗ trợ (Sửa chữa, Vệ sinh, Điện, Nước...)
        /// </summary>
        public int LoaiHoTroId { get; set; }
        
        /// <summary>
        /// Tiêu đề yêu cầu
        /// </summary>
        public string TieuDe { get; set; }
        
        /// <summary>
        /// Mô tả chi tiết
        /// </summary>
        public string MoTa { get; set; }
        
        /// <summary>
        /// Trạng thái: Moi / DangXuLy / HoanThanh / TuChoi
        /// </summary>
        public string TrangThai { get; set; }
        
        /// <summary>
        /// Thời gian tạo yêu cầu
        /// </summary>
        public DateTimeOffset? ThoiGianTao { get; set; }
        
        /// <summary>
        /// ID người gửi yêu cầu (Người thuê)
        /// </summary>
        public Guid NguoiYeuCau { get; set; }
        
        /// <summary>
        /// Số thứ tự yêu cầu (để hiển thị)
        /// </summary>
        public int SoYeuCau { get; set; }
        
        // === Thông tin bổ sung (nếu API trả về) ===
        
        /// <summary>
        /// Tên loại hỗ trợ
        /// </summary>
        public string TenLoaiHoTro { get; set; }
        
        /// <summary>
        /// Tên người yêu cầu
        /// </summary>
        public string TenNguoiYeuCau { get; set; }
        
        /// <summary>
        /// Tiêu đề phòng (nếu có)
        /// </summary>
        public string TenPhong { get; set; }
    }
}
