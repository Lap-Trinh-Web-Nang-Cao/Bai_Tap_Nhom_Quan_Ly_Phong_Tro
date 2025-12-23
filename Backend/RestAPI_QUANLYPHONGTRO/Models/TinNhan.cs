using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("TinNhan")]
    public class TinNhan
    {
        [Key]
        public Guid TinNhanId { get; set; }

        [Required]
        public Guid FromUser { get; set; }

        [Required]
        public Guid ToUser { get; set; }

        [MaxLength]
        public string? NoiDung { get; set; }

        public Guid? TapTinId { get; set; }

        public DateTimeOffset? ThoiGian { get; set; }

        [Required]
        public bool DaDoc { get; set; }

        // ===== MỚI: HỖ TRỢ SUPPORT & AI =====

        /// <summary>
        /// Loại tin nhắn: "Normal" (người-người), "Support" (hỗ trợ), "AI" (AI trả lời)
        /// </summary>
        [MaxLength(50)]
        public string LoaiTinNhan { get; set; } = "Normal";

        /// <summary>
        /// Liên kết với yêu cầu hỗ trợ (nếu LoaiTinNhan = Support)
        /// </summary>
        public Guid? YeuCauHoTroId { get; set; }

        /// <summary>
        /// Ưu tiên: "Low" / "Medium" / "High" (dùng cho support tickets)
        /// </summary>
        [MaxLength(50)]
        public string? UuTien { get; set; }

        /// <summary>
        /// Loại vấn đề: "Sửa chữa", "Thanh toán", "Khác", "Chung"
        /// </summary>
        [MaxLength(100)]
        public string? LoaiVanDe { get; set; }

        /// <summary>
        /// Dữ liệu meta cho AI (JSON): { "confidence": 0.95, "intent": "maintenance", ... }
        /// </summary>
        [MaxLength]
        public string? MetaData { get; set; }

        /// <summary>
        /// TRUE = tin nhắn này từ AI hoặc bot tự động trả lời
        /// </summary>
        public bool IsAIResponse { get; set; } = false;

        /// <summary>
        /// ID ticket hỗ trợ (để AI tìm context)
        /// </summary>
        public int? SupportTicketId { get; set; }

        /// <summary>
        /// Trạng thái: "Pending" (chờ xử lý), "Answered" (đã trả lời), "Resolved" (giải quyết)
        /// </summary>
        [MaxLength(50)]
        public string? TrangThaiHoTro { get; set; }
    }
}