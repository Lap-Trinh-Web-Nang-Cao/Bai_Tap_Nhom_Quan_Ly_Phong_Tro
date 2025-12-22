using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web; // Dùng HttpPostedFileBase cho MVC 5
using System.Web.Mvc;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class TaoPhongViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn Khu trọ")]
        public Guid NhaTroId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề phòng")]
        [Display(Name = "Tiêu đề phòng")]
        public string TieuDe { get; set; }

        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Giá tiền phải lớn hơn 0")]
        public long GiaTien { get; set; }

        public decimal? DienTich { get; set; }
        public long? TienCoc { get; set; }
        public int? SoNguoiToiDa { get; set; }

        // File ảnh upload từ View
        [Display(Name = "Ảnh đại diện")]
        public HttpPostedFileBase HinhAnhUpload { get; set; }

        // Dropdown data
        public List<SelectListItem> DanhSachNhaTro { get; set; }

        public TaoPhongViewModel()
        {
            DanhSachNhaTro = new List<SelectListItem>();
        }
    }
}