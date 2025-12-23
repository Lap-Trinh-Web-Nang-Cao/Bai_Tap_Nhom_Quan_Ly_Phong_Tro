using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    /// <summary>
    /// Request tạo ticket hỗ trợ
    /// </summary>
    public class CreateSupportTicketRequest
    {
        [Required]
        [MaxLength(300)]
        public string TieuDe { get; set; }

        [Required]
        [MaxLength(2000)]
        public string MoTa { get; set; }

        /// <summary>
        /// Loại vấn đề: "Sửa chữa", "Thanh toán", "Khác", "Chung"
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LoaiVanDe { get; set; }

        /// <summary>
        /// Ưu tiên: "Low", "Medium", "High"
        /// </summary>
        [MaxLength(50)]
        public string UuTien { get; set; } = "Medium";

        /// <summary>
        /// Liên kết đến phòng (nếu có)
        /// </summary>
        public Guid? PhongId { get; set; }

        /// <summary>
        /// Cần hỗ trợ trực tiếp từ admin không?
        /// </summary>
        public bool CanhAdminHoTro { get; set; } = false;
    }

    /// <summary>
    /// Request gửi tin nhắn trong support ticket
    /// </summary>
    public class SendSupportMessageRequest
    {
        [Required]
        [MaxLength(2000)]
        public string NoiDung { get; set; }

        /// <summary>
        /// File đính kèm (nếu có)
        /// </summary>
        public Guid? TapTinId { get; set; }

        /// <summary>
        /// Metadata cho AI (JSON string): { "intent": "...", "entities": [...] }
        /// </summary>
        public string? MetaData { get; set; }
    }

    /// <summary>
    /// Response tin nhắn hỗ trợ
    /// </summary>
    public class SupportMessageResponse
    {
        public Guid TinNhanId { get; set; }
        public Guid FromUser { get; set; }
        public Guid ToUser { get; set; }
        public string NoiDung { get; set; }
        public DateTimeOffset ThoiGian { get; set; }
        public bool DaDoc { get; set; }
        public string LoaiTinNhan { get; set; }
        public bool IsAIResponse { get; set; }
        public string? TrangThaiHoTro { get; set; }

        /// <summary>
        /// Info người gửi (bỏ password)
        /// </summary>
        public object SenderInfo { get; set; }
    }

    /// <summary>
    /// Thông tin ticket hỗ trợ
    /// </summary>
    public class SupportTicketResponse
    {
        public Guid TicketId { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public string LoaiVanDe { get; set; }
        public string UuTien { get; set; }
        public string TrangThai { get; set; }
        public Guid NguoiGui { get; set; }
        public int SoTinNhan { get; set; }
        public int SoTinChuaDangXem { get; set; }
        public DateTimeOffset ThoiGianTao { get; set; }
        public DateTimeOffset? ThoiGianCapNhat { get; set; }
        public List<SupportMessageResponse> TinNhanGanDay { get; set; }
    }
}