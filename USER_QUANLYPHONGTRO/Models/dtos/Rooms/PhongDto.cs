using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Rooms
{
    /// <summary>
    /// DTO cho thông tin phòng - dùng khi gọi API từ Backend
    /// </summary>
    public class PhongDto
    {
        /// <summary>
        /// ID phòng (GUID)
        /// </summary>
        public Guid PhongId { get; set; }

        /// <summary>
        /// ID nhà trọ chứa phòng
        /// </summary>
        public Guid NhaTroId { get; set; }

        /// <summary>
        /// Tiêu đề/tên phòng (vd: Phòng 101, Phòng VIP...)
        /// </summary>
        public string TieuDe { get; set; }

        /// <summary>
        /// Diện tích phòng (m²)
        /// </summary>
        public decimal? DienTich { get; set; }

        /// <summary>
        /// Giá tiền thuê/tháng (VND)
        /// </summary>
        public long GiaTien { get; set; }

        /// <summary>
        /// Tiền cọc/đặt cọc (VND)
        /// </summary>
        public long? TienCoc { get; set; }

        /// <summary>
        /// Số người tối đa có thể thuê
        /// </summary>
        public int? SoNguoiToiDa { get; set; }

        /// <summary>
        /// Trạng thái phòng (con_trong, da_thue, dang_sua_chua...)
        /// </summary>
        public string TrangThai { get; set; }

        /// <summary>
        /// Điểm đánh giá trung bình (1-5 sao)
        /// </summary>
        public double? DiemTrungBinh { get; set; }

        /// <summary>
        /// Số lượng đánh giá
        /// </summary>
        public int? SoLuongDanhGia { get; set; }

        /// <summary>
        /// Hình ảnh đại diện phòng (URL)
        /// </summary>
        public string HinhAnhDaiDien { get; set; }

        /// <summary>
        /// Thông tin nhà trọ
        /// </summary>
        public NhaTroDto NhaTro { get; set; }

        /// <summary>
        /// Danh sách tiện ích
        /// </summary>
        public List<TienIchDto> TienIchs { get; set; } = new List<TienIchDto>();

        /// <summary>
        /// Mô tả chi tiết phòng
        /// </summary>
        public string MoTa { get; set; }

        /// <summary>
        /// Danh sách hình ảnh chi tiết
        /// </summary>
        public List<string> DanhSachHinhAnh { get; set; } = new List<string>();

        /// <summary>
        /// Ngày tạo phòng
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Ngày cập nhật cuối cùng
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Đã được duyệt bởi Admin?
        /// </summary>
        public bool IsDuyet { get; set; }

        /// <summary>
        /// Bị khóa?
        /// </summary>
        public bool IsBiKhoa { get; set; }

        /// <summary>
        /// Xóa hay chưa?
        /// </summary>
        public bool IsDeleted { get; set; }
    }

    // NOTE:
    // `NhaTroDto` is declared in `Models/Dtos/Rooms/NhaTroDto.cs`.
    // `TienIchDto` is declared in `Models/Dtos/Rooms/TienIchDto.cs`.
}
