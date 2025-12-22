using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ApplicationDbContext _context;
        private static Dictionary<string, string> _cache = new(); // Simple in-memory cache

        public SystemSettingService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Get Methods

        public async Task<List<SystemSettingResponse>> GetAllSettingsAsync()
        {
            return await _context.SystemSettings
                .Where(s => s.IsVisible)
                .Select(s => new SystemSettingResponse
                {
                    SettingId = s.SettingId,
                    SettingKey = s.SettingKey,
                    SettingValue = s.SettingValue,
                    DataType = s.DataType,
                    Description = s.Description,
                    GroupName = s.GroupName,
                    IsVisible = s.IsVisible,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<List<SystemSettingResponse>> GetSettingsByGroupAsync(string groupName)
        {
            return await _context.SystemSettings
                .Where(s => s.GroupName == groupName && s.IsVisible)
                .Select(s => new SystemSettingResponse
                {
                    SettingId = s.SettingId,
                    SettingKey = s.SettingKey,
                    SettingValue = s.SettingValue,
                    DataType = s.DataType,
                    Description = s.Description,
                    GroupName = s.GroupName,
                    IsVisible = s.IsVisible,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<SystemSettingResponse> GetSettingByKeyAsync(string key)
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == key);

            if (setting == null) return null;

            return new SystemSettingResponse
            {
                SettingId = setting.SettingId,
                SettingKey = setting.SettingKey,
                SettingValue = setting.SettingValue,
                DataType = setting.DataType,
                Description = setting.Description,
                GroupName = setting.GroupName,
                IsVisible = setting.IsVisible,
                CreatedAt = setting.CreatedAt,
                UpdatedAt = setting.UpdatedAt
            };
        }

        #endregion

        #region Create/Update Methods

        public async Task<ServiceResult<SystemSettingResponse>> CreateSettingAsync(SystemSettingRequest request)
        {
            try
            {
                // Check duplicate
                var existing = await _context.SystemSettings
                    .FirstOrDefaultAsync(s => s.SettingKey == request.SettingKey);

                if (existing != null)
                    return new ServiceResult<SystemSettingResponse>
                    {
                        Success = false,
                        Message = "Cài đặt với khóa này đã tồn tại"
                    };

                var setting = new SystemSetting
                {
                    SettingKey = request.SettingKey,
                    SettingValue = request.SettingValue,
                    DataType = request.DataType ?? "string",
                    Description = request.Description,
                    GroupName = request.GroupName ?? "general",
                    IsVisible = request.IsVisible,
                    CreatedAt = DateTimeOffset.Now
                };

                _context.SystemSettings.Add(setting);
                await _context.SaveChangesAsync();

                // Clear cache
                _cache.Clear();

                return new ServiceResult<SystemSettingResponse>
                {
                    Success = true,
                    Message = "Tạo cài đặt thành công",
                    Data = new SystemSettingResponse
                    {
                        SettingId = setting.SettingId,
                        SettingKey = setting.SettingKey,
                        SettingValue = setting.SettingValue,
                        DataType = setting.DataType,
                        Description = setting.Description,
                        GroupName = setting.GroupName,
                        IsVisible = setting.IsVisible,
                        CreatedAt = setting.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<SystemSettingResponse>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<SystemSettingResponse>> UpdateSettingAsync(int id, SystemSettingRequest request)
        {
            try
            {
                var setting = await _context.SystemSettings.FindAsync(id);
                if (setting == null)
                    return new ServiceResult<SystemSettingResponse>
                    {
                        Success = false,
                        Message = "Không tìm thấy cài đặt"
                    };

                setting.SettingValue = request.SettingValue;
                setting.DataType = request.DataType ?? setting.DataType;
                setting.Description = request.Description;
                setting.GroupName = request.GroupName ?? setting.GroupName;
                setting.IsVisible = request.IsVisible;
                setting.UpdatedAt = DateTimeOffset.Now;

                await _context.SaveChangesAsync();

                // Clear cache
                _cache.Clear();

                return new ServiceResult<SystemSettingResponse>
                {
                    Success = true,
                    Message = "Cập nhật cài đặt thành công",
                    Data = new SystemSettingResponse
                    {
                        SettingId = setting.SettingId,
                        SettingKey = setting.SettingKey,
                        SettingValue = setting.SettingValue,
                        DataType = setting.DataType,
                        Description = setting.Description,
                        GroupName = setting.GroupName,
                        IsVisible = setting.IsVisible,
                        UpdatedAt = setting.UpdatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<SystemSettingResponse>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<SystemSettingResponse>> UpdateSettingByKeyAsync(string key, string value)
        {
            try
            {
                var setting = await _context.SystemSettings
                    .FirstOrDefaultAsync(s => s.SettingKey == key);

                if (setting == null)
                    return new ServiceResult<SystemSettingResponse>
                    {
                        Success = false,
                        Message = "Không tìm thấy cài đặt"
                    };

                setting.SettingValue = value;
                setting.UpdatedAt = DateTimeOffset.Now;

                await _context.SaveChangesAsync();

                // Clear cache
                _cache.Remove(key);

                return new ServiceResult<SystemSettingResponse>
                {
                    Success = true,
                    Message = "Cập nhật thành công"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<SystemSettingResponse>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        #endregion

        #region Delete Methods

        public async Task<ServiceResult<bool>> DeleteSettingAsync(int id)
        {
            try
            {
                var setting = await _context.SystemSettings.FindAsync(id);
                if (setting == null)
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        Message = "Không tìm thấy cài đặt"
                    };

                _context.SystemSettings.Remove(setting);
                await _context.SaveChangesAsync();

                // Clear cache
                _cache.Clear();

                return new ServiceResult<bool>
                {
                    Success = true,
                    Message = "Xóa cài đặt thành công",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        #endregion

        #region Helper Methods

        public string GetSettingValue(string key, string defaultValue = "")
        {
            // Check cache first
            if (_cache.TryGetValue(key, out var cachedValue))
                return cachedValue;

            var setting = _context.SystemSettings
                .FirstOrDefault(s => s.SettingKey == key);

            if (setting == null)
                return defaultValue;

            _cache[key] = setting.SettingValue ?? defaultValue;
            return _cache[key];
        }

        public int GetSettingValueAsInt(string key, int defaultValue = 0)
        {
            var value = GetSettingValue(key);
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        public bool GetSettingValueAsBool(string key, bool defaultValue = false)
        {
            var value = GetSettingValue(key);
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }

        public decimal GetSettingValueAsDecimal(string key, decimal defaultValue = 0)
        {
            var value = GetSettingValue(key);
            return decimal.TryParse(value, out var result) ? result : defaultValue;
        }

        #endregion
    }
}
