using System;
using System.Collections.Generic;

namespace ADMIN_QUANLYPHONGTRO.Models.ViewModels
{
    /// <summary>
    /// ViewModel cho danh sách chủ trọ chờ xét duyệt
    /// </summary>
    public class PendingHostsViewModel
    {
        public List<PendingHostItem> PendingHosts { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SearchKeyword { get; set; }
        public string StatusFilter { get; set; }

        public PendingHostsViewModel()
        {
            PendingHosts = new List<PendingHostItem>();
            PageSize = 10;
            CurrentPage = 1;
        }
    }

    /// <summary>
    /// Item đại diện cho một chủ trọ chờ duyệt
    /// </summary>
    public class PendingHostItem
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Thông tin pháp lý
        public string CCCD { get; set; }
        public DateTime? NgayCapCCCD { get; set; }
        public string NoiCapCCCD { get; set; }
        public string DiaChiThuongTru { get; set; }
        public string TrangThaiXacThuc { get; set; } // "ChoDuyet", "DaDuyet", "TuChoi"
        
        // Files đính kèm
        public List<DocumentFile> Documents { get; set; }
        public bool HasDocuments => Documents != null && Documents.Count > 0;

        public PendingHostItem()
        {
            Documents = new List<DocumentFile>();
        }
    }

    /// <summary>
    /// Tài liệu đính kèm (CCCD, Giấy phép KD...)
    /// </summary>
    public class DocumentFile
    {
        public Guid TapTinId { get; set; }
        public string TenTapTin { get; set; }
        public string LoaiTapTin { get; set; } // "CCCD_MATTRUOC", "CCCD_MATSAU", "GPKD"
        public string DuongDan { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    /// <summary>
    /// Request khi xét duyệt hoặc từ chối
    /// </summary>
    public class VerifyHostRequest
    {
        public Guid NguoiDungId { get; set; }
        public bool IsApproved { get; set; }
        public string LyDoTuChoi { get; set; }
        public string GhiChu { get; set; }
    }
}
