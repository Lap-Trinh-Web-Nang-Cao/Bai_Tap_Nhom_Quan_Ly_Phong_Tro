using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Bookings
{
    public class CreateDatPhongDto
    {
        // SỬA: Đổi "phongId" -> "PhongId"
        [JsonProperty("PhongId")]
        public Guid PhongId { get; set; }

        // SỬA: Đổi "chuTroId" -> "ChuTroId"
        [JsonProperty("ChuTroId")]
        public Guid ChuTroId { get; set; }

        // SỬA: Đổi "loai" -> "Loai"
        [JsonProperty("Loai")]
        public string Loai { get; set; }

        // SỬA: Đổi "batDau" -> "BatDau"
        [JsonProperty("BatDau")]
        public DateTimeOffset BatDau { get; set; }

        // SỬA: Đổi "ketThuc" -> "KetThuc"
        [JsonProperty("KetThuc")]
        public DateTimeOffset? KetThuc { get; set; }

        // SỬA: Đổi "ghiChu" -> "GhiChu"
        [JsonProperty("GhiChu")]
        public string GhiChu { get; set; }
    }
}