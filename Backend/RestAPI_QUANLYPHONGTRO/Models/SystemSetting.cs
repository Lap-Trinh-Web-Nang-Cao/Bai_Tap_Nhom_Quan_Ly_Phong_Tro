using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    /// <summary>
    /// Cấu hình hệ thống
    /// </summary>
    [Table("SystemSettings")]
    public class SystemSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SettingId { get; set; }

        /// <summary>
        /// Khóa cài đặt (định danh duy nhất)
        /// Ví dụ: "app.name", "service.fee", "support.hotline"
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string SettingKey { get; set; }

        /// <summary>
        /// Giá trị cài đặt (lưu dạng string, convert khi cần)
        /// </summary>
        public string? SettingValue { get; set; }

        /// <summary>
        /// Loại dữ liệu: string, integer, boolean, decimal
        /// </summary>
        [MaxLength(50)]
        public string? DataType { get; set; } = "string";

        /// <summary>
        /// Mô tả cài đặt
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Nhóm cài đặt: general, service, security, appearance
        /// </summary>
        [MaxLength(100)]
        public string? GroupName { get; set; } = "general";

        /// <summary>
        /// Có được hiển thị hay không
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Thời gian cập nhật
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
}
