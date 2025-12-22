using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class DatLichViewModel
    {
        public Guid PhongId { get; set; }
        public string TieuDePhong { get; set; }
        public string DiaChiPhong { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian xem phòng")]
        [Display(Name = "Thời gian xem phòng")]
        public DateTime ThoiGianXem { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }
    }
}