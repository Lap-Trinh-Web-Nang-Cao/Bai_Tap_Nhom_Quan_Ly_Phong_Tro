namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class CreateSupportTicketRequest
    {
        public string TieuDe { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public int? UuTien { get; set; }
        public string? LoaiVanDe { get; set; }
    }
}