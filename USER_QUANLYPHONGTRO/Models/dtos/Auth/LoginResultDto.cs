using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos
{
    public class LoginResultDto
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; } // Có thể là tên ("CHUTRO") hoặc mã số ("2")
    }
}