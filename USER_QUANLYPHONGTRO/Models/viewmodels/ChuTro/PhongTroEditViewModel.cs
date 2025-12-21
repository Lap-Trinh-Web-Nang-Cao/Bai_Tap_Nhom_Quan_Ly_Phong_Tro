using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;       // Dùng cho HttpPostedFileBase (Upload ảnh)
using System.Web.Mvc;   // QUAN TRỌNG: Dùng cho SelectListItem (Dropdown/Checkbox)

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class PhongTroEditViewModel

    {
      
         
        
        public Guid? PhongId { get; set; } // null = tạo mới

        // --- GIỮ LẠI CỦA BẠN (Data Annotations rất tốt) ---
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [Display(Name = "Tiêu đề phòng")]
        public string TieuDe { get; set; }

        [Display(Name = "Diện tích (m²)")]
        public decimal? DienTich { get; set; } // DB là Decimal -> C# là decimal

        [Required]
        [Display(Name = "Giá thuê / tháng")]
        public long GiaTien { get; set; }      // DB là BigInt -> C# là long

        [Display(Name = "Tiền cọc")]
        public long? TienCoc { get; set; }

        [Display(Name = "Số người tối đa")]
        public int SoNguoiToiDa { get; set; }

        // --- PHẦN BỔ SUNG ĐỂ KHỚP VỚI DATABASE VÀ VIEW ---

        // 1. Thêm Địa chỉ (Vì Database bảng NhaTro có cột DiaChi)
        [Display(Name = "Địa chỉ chi tiết")]
        public string DiaChi { get; set; }

        // 2. Sửa lại Tiện ích để khớp với View checkbox
        // (Thay IDictionary bằng List<SelectListItem> là chuẩn MVC để render checkbox)
        public List<SelectListItem> AvailableAmenities { get; set; }

        // (Thay IList<int> bằng string[] hoặc int[] để hứng dữ liệu từ form submit)
        public string[] SelectedAmenities { get; set; }

        // 3. Thêm phần Upload ảnh
        [Display(Name = "Chọn ảnh tải lên")]
        public HttpPostedFileBase[] ImageFiles { get; set; }

        // (Để hiển thị ảnh cũ khi bấm Sửa)
        public string HinhAnh { get; set; }

        [Display(Name = "Trạng thái phòng")]
        public string TrangThai { get; set; }
    }
}