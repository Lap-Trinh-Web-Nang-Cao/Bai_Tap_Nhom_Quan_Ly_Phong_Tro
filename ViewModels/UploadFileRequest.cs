using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class UploadFileRequest
    {
        [Required]
        public IFormFile File { get; set; } // Đối tượng file gửi lên
    }
}
