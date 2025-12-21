using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ISystemSettingService
    {
        // Get
        Task<List<SystemSettingResponse>> GetAllSettingsAsync();
        Task<List<SystemSettingResponse>> GetSettingsByGroupAsync(string groupName);
        Task<SystemSettingResponse> GetSettingByKeyAsync(string key);

        // Create
        Task<ServiceResult<SystemSettingResponse>> CreateSettingAsync(SystemSettingRequest request);

        // Update
        Task<ServiceResult<SystemSettingResponse>> UpdateSettingAsync(int id, SystemSettingRequest request);
        Task<ServiceResult<SystemSettingResponse>> UpdateSettingByKeyAsync(string key, string value);

        // Delete
        Task<ServiceResult<bool>> DeleteSettingAsync(int id);

        // Helper
        string GetSettingValue(string key, string defaultValue = "");
        int GetSettingValueAsInt(string key, int defaultValue = 0);
        bool GetSettingValueAsBool(string key, bool defaultValue = false);
        decimal GetSettingValueAsDecimal(string key, decimal defaultValue = 0);
    }
}
