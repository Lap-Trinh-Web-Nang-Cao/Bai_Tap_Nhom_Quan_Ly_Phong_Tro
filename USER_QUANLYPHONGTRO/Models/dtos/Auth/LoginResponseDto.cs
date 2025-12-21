using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Auth
{
    public class LoginResponseDto
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("nguoiDungId")]
        public Guid NguoiDungId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        // Quan trọng: Map đúng tên biến JSON từ API trả về
        [JsonProperty("vaiTroId")]
        public int VaiTroId { get; set; }

        [JsonProperty("hoTen")]
        public string HoTen { get; set; }
    }
}