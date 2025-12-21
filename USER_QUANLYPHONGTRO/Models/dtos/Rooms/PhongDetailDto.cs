using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Rooms
{
    public class PhongDetailDto
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
        public string MoTa { get; set; }
        public string TrangThai { get; set; } // "ConTrong", "DaThue"

        public string AnhDaiDien { get; set; }

        // Map thông tin nhà trọ
        [JsonProperty("nhaTro")]
        public NhaTroDto NhaTro { get; set; }

        // Danh sách tiện ích (Wifi, Tủ lạnh...) - Nếu API trả về
        [JsonProperty("tienIchs")]
        public List<string> TienIchs { get; set; }

        // Danh sách hình ảnh chi tiết (Nếu API trả về List<TapTin>)
        // Hiện tại tạm thời chúng ta sẽ fake list này từ AnhDaiDien nếu Backend chưa có bảng ảnh riêng
        public List<string> DanhSachAnh { get; set; }
    }
}