using System;
using System.Collections.Generic;

namespace ADMIN_QUANLYPHONGTRO.Models.ViewModels
{
    // ====== API RESPONSE DTOs ======
    /// <summary>
    /// DTO Response từ Backend API - Danh sách chủ trọ chờ duyệt
    /// </summary>
    public class HostPendingListResponse
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int totalCount { get; set; }
        public List<HostPendingItemDto> items { get; set; }
    }

    /// <summary>
    /// DTO item từ Backend API
    /// </summary>
    public class HostPendingItemDto
    {
        public Guid nguoiDungId { get; set; }
        public string hoTen { get; set; }
        public string email { get; set; }
        public string dienThoai { get; set; }
        public string avatar { get; set; }
        public string soCCCD { get; set; }
        public string loaiGiayTo { get; set; }
        public int soTapTinDinhKem { get; set; }
        public bool daTaiGiayTo { get; set; }
        public DateTime ngayDangKy { get; set; }
        public string trangThaiXacThuc { get; set; }
    }

    /// <summary>
    /// DTO Response từ Backend API - Chi tiết chủ trọ
    /// </summary>
    public class HostApprovalItemDto
    {
        public Guid nguoiDungId { get; set; }
        public string hoTen { get; set; }
        public string email { get; set; }
        public string dienThoai { get; set; }
        public DateTime ngaySinh { get; set; }
        public string queQuan { get; set; }
        public string soCCCD { get; set; }
        public string avatar { get; set; }
        public string cccdMatTruocUrl { get; set; }
        public string cccdMatSauUrl { get; set; }
        public string giayPhepKinhDoanhUrl { get; set; }
        public string trangThaiXacThuc { get; set; }
    }

    // ====== VIEW MODELS ======
    /// <summary>
    /// ViewModel cho item trong danh sách chủ trọ chờ duyệt
    /// </summary>
    public class HostPendingItemViewModel
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string SoCCCD { get; set; }
        public string Avatar { get; set; }
        public DateTime NgayDangKy { get; set; }
        public bool DaTaiGiayTo { get; set; }
        public string TrangThaiXacThuc { get; set; }
        public string LoaiGiayTo { get; set; }
        public int SoTapTinDinhKem { get; set; }
    }

    /// <summary>
    /// ViewModel chi tiết chủ trọ để duyệt (sử dụng trong Modal)
    /// </summary>
    public class HostApprovalDetailViewModel
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public DateTime NgaySinh { get; set; }
        public string QueQuan { get; set; }
        public string SoCCCD { get; set; }
        public string Avatar { get; set; }

        // Ảnh giấy tờ
        public string CCCDMatTruocUrl { get; set; }
        public string CCCDMatSauUrl { get; set; }
        public string GiayPhepKinhDoanhUrl { get; set; }

        // Trạng thái hiện tại
        public string TrangThaiXacThuc { get; set; }
    }

    /// <summary>
    /// ViewModel cho item người dùng
    /// </summary>
    public class UserItemViewModel
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string Avatar { get; set; }
        public int VaiTroId { get; set; }
        public string VaiTroName { get; set; }
        public bool IsKhoa { get; set; }
        public bool IsEmailXacThuc { get; set; }
        public DateTime NgayDangKy { get; set; }
        public int SoPhongDaThue { get; set; } // Cho người thuê
        public int SoPhongDaDang { get; set; } // Cho chủ trọ
    }

    /// <summary>
    /// ViewModel cho item báo cáo vi phạm
    /// </summary>
    public class ReportItemViewModel
    {
        public Guid BaoCaoId { get; set; }
        public string NguoiBaoCaoName { get; set; }
        public string NguoiBaoCaoAvatar { get; set; }
        public string DoiTuongType { get; set; } // "Phòng" / "Người dùng"
        public string DoiTuongName { get; set; }
        public Guid? DoiTuongId { get; set; }
        public string LyDoViPham { get; set; }
        public string MoTaChiTiet { get; set; }
        public DateTime NgayBaoCao { get; set; }
        public string TrangThai { get; set; }
        public string MucDo { get; set; } // "Cao", "Trung bình", "Thấp"
    }

    /// <summary>
    /// ViewModel cho item giao dịch
    /// </summary>
    public class TransactionItemViewModel
    {
        public Guid BienLaiId { get; set; }
        public string MaGiaoDich { get; set; }
        public string NguoiThanhToanName { get; set; }
        public string NguoiThanhToanEmail { get; set; }
        public decimal SoTien { get; set; }
        public string PhongName { get; set; }
        public Guid? PhongId { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public bool DaXacNhan { get; set; }
        public string MinhChungUrl { get; set; }
        public string GhiChu { get; set; }
    }

    /// <summary>
    /// ViewModel cho category item (Tiện ích, Quận huyện, etc)
    /// </summary>
    public class CategoryItemViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public int SoLuongSuDung { get; set; } // Số phòng đang dùng
        public DateTime? NgayTao { get; set; }
    }
}
