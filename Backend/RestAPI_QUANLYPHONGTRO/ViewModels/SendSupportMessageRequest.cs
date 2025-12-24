namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class SendSupportMessageRequest
    {
        public string NoiDung { get; set; } = string.Empty;
        public Guid? TapTinId { get; set; }
        public string? MetaData { get; set; }
    }
}