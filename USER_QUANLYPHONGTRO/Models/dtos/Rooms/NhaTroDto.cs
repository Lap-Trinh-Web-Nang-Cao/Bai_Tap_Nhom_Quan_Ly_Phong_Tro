using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Rooms
{
    public class NhaTroDto
    {
        [JsonProperty("nhaTroId")]
        public Guid NhaTroId { get; set; }

        [JsonProperty("tieuDe")]
        public string TieuDe { get; set; }

        [JsonProperty("diaChi")]
        public string DiaChi { get; set; }

        // --- QUAN TRỌNG: Thêm trường này ---
        [JsonProperty("chuTroId")]
        public Guid ChuTroId { get; set; }
    }
}