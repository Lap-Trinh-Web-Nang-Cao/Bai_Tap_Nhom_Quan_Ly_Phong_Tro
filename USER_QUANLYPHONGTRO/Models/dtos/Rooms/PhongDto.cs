using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Rooms
{
    public class PhongDto
    {
        public Guid PhongId { get; set; }
        [JsonProperty("ChuTroId")]
        public Guid ChuTroId { get; set; }

        [JsonProperty("tenPhong")]
        public string TenPhong { get; set; }

        [JsonProperty("tieuDe")]
        public string TieuDe { get; set; }

        public decimal GiaTien { get; set; }
        public float DienTich { get; set; }
        public string AnhDaiDien { get; set; }

        // Có thể API trả về DiaChi ở ngoài (flat)
        [JsonProperty("diaChi")]
        public string DiaChi { get; set; }

        // HOẶC API trả về TenNhaTro ở ngoài
        [JsonProperty("tenNhaTro")]
        public string TenNhaTro { get; set; }

        // --- QUAN TRỌNG: Thêm object NhaTro để lấy thông tin chi tiết nếu ở ngoài bị null ---
        [JsonProperty("nhaTro")]
        public NhaTroDto NhaTro { get; set; }
    }
}