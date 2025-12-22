using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class PhongTroEditViewModel
    {
        public Guid? PhongId { get; set; }   // null = tạo mới

        [Required]
        [Display(Name = "Tiêu đề phòng")]
        public string TieuDe { get; set; }

        [Display(Name = "Diện tích (m²)")]
        public decimal? DienTich { get; set; }

        [Required]
        [Display(Name = "Giá thuê / tháng")]
        public long GiaTien { get; set; }

        [Display(Name = "Tiền cọc")]
        public long? TienCoc { get; set; }

        [Display(Name = "Số người tối đa")]
        public int SoNguoiToiDa { get; set; }

        public IList<int> SelectedTienIchIds { get; set; }
        public IDictionary<int, string> AllTienIchOptions { get; set; }
    }
}