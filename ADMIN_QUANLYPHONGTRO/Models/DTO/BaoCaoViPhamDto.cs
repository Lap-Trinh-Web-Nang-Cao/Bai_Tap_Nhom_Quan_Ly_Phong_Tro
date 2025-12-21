using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    /// <summary>
    /// DTO cho báo cáo vi phạm - mapping từ API Backend
    /// </summary>
    public class BaoCaoViPhamDto
    {
        public Guid BaoCaoId { get; set; }
        
        /// <summary>
        /// Loại thực thể bị báo cáo: PHONG / NGUOIDUNG / DANHGIA
        /// </summary>
        public string LoaiThucThe { get; set; }
        
        /// <summary>
        /// ID của thực thể bị báo cáo (PhongId hoặc NguoiDungId)
        /// </summary>
        public Guid? ThucTheId { get; set; }
        
        /// <summary>
        /// ID người gửi báo cáo
        /// </summary>
        public Guid NguoiBaoCao { get; set; }
        
        /// <summary>
        /// ID loại vi phạm (foreign key đến bảng ViPham)
        /// </summary>
        public int? ViPhamId { get; set; }
        
        /// <summary>
        /// Tiêu đề báo cáo
        /// </summary>
        public string TieuDe { get; set; }
        
        /// <summary>
        /// Nội dung mô tả chi tiết
        /// </summary>
        public string MoTa { get; set; }
        
        /// <summary>
        /// Trạng thái: CHO_XU_LY / DANG_XU_LY / DA_XU_LY / TU_CHOI
        /// </summary>
        public string TrangThai { get; set; }
        
        /// <summary>
        /// Kết quả xử lý
        /// </summary>
        public string KetQua { get; set; }
        
        /// <summary>
        /// ID admin/moderator xử lý
        /// </summary>
        public Guid? NguoiXuLy { get; set; }
        
        /// <summary>
        /// Thời gian gửi báo cáo
        /// </summary>
        public DateTimeOffset? ThoiGianBaoCao { get; set; }
        
        /// <summary>
        /// Thời gian xử lý
        /// </summary>
        public DateTimeOffset? ThoiGianXuLy { get; set; }
        
        /// <summary>
        /// Số thứ tự báo cáo
        /// </summary>
        public int SoBaoCao { get; set; }
    }

    /// <summary>
    /// DTO cho loại vi phạm
    /// </summary>
    public class ViPhamDto
    {
        public int ViPhamId { get; set; }
        public string TenViPham { get; set; }
        public string MoTa { get; set; }
        public long? HinhPhatTien { get; set; }
        public int? SoDiemTru { get; set; }
    }

    /// <summary>
    /// Request model để xử lý báo cáo
    /// </summary>
    public class XuLyBaoCaoRequest
    {
        public string TrangThai { get; set; }
        public string KetQua { get; set; }
        public Guid? NguoiXuLy { get; set; }
    }
}
