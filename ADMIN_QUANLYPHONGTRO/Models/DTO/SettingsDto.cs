namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    /// <summary>
    /// DTO cho Tiện ích
    /// </summary>
    public class TienIchDto
    {
        public int TienIchId { get; set; }
        public string Ten { get; set; }
    }

    /// <summary>
    /// DTO cho Quận/Huyện
    /// </summary>
    public class QuanHuyenDto
    {
        public int QuanHuyenId { get; set; }
        public string Ten { get; set; }
    }

    /// <summary>
    /// DTO cho Phường
    /// </summary>
    public class PhuongDto
    {
        public int PhuongId { get; set; }
        public int QuanHuyenId { get; set; }
        public string Ten { get; set; }
        
        // Thông tin bổ sung (nếu API trả về)
        public string QuanHuyenTen { get; set; }
    }
}
