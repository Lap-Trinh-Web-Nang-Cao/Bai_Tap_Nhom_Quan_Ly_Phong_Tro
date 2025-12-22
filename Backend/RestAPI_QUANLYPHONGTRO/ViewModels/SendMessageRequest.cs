using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class SendMessageRequest
    {
        [Required]
        public Guid ToUser { get; set; } // Gửi cho ai

        public string? NoiDung { get; set; }

        public Guid? TapTinId { get; set; } // ID file (nếu có upload ảnh)
    }
}
