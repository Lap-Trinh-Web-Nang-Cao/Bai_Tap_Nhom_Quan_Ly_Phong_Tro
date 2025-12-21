using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class DeviceTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
