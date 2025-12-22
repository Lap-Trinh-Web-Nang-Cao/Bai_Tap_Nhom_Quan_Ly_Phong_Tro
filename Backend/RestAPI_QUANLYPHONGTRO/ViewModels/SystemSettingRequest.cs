namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class SystemSettingRequest
    {
        public string SettingKey { get; set; }
        public string? SettingValue { get; set; }
        public string? DataType { get; set; } = "string";
        public string? Description { get; set; }
        public string? GroupName { get; set; } = "general";
        public bool IsVisible { get; set; } = true;
    }

    public class SystemSettingResponse
    {
        public Guid SettingId { get; set; }
        public string SettingKey { get; set; }
        public string? SettingValue { get; set; }
        public string? DataType { get; set; }
        public string? Description { get; set; }
        public string? GroupName { get; set; }
        public bool IsVisible { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
}
