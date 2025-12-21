using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;

namespace RestAPI_QUANLYPHONGTRO.Seed
{
    public static class DatabaseInitializer
    {
        public static void InitializeDatabase(ApplicationDbContext context)
        {
            // Tạo bảng nếu chưa tồn tại
            context.Database.EnsureCreated();

            // Seed SystemSettings
            SeedSystemSettings(context);
        }

        private static void SeedSystemSettings(ApplicationDbContext context)
        {
            // Kiểm tra xem đã có settings chưa
            if (context.SystemSettings.Any())
                return;

            var defaultSettings = new List<SystemSetting>
            {
                // General Info
                new SystemSetting { SettingKey = "app.name", SettingValue = "Quản Lý Phòng Trọ", DataType = "string", GroupName = "general", Description = "Tên ứng dụng", IsVisible = true },
                new SystemSetting { SettingKey = "app.description", SettingValue = "Ứng dụng quản lý phòng trọ toàn diện", DataType = "string", GroupName = "general", Description = "Mô tả ứng dụng", IsVisible = true },
                new SystemSetting { SettingKey = "app.url", SettingValue = "https://example.com", DataType = "string", GroupName = "general", Description = "URL ứng dụng", IsVisible = true },

                // Contact Info
                new SystemSetting { SettingKey = "support.hotline", SettingValue = "0123 456 789", DataType = "string", GroupName = "contact", Description = "Hotline hỗ trợ", IsVisible = true },
                new SystemSetting { SettingKey = "support.email", SettingValue = "support@example.com", DataType = "string", GroupName = "contact", Description = "Email hỗ trợ", IsVisible = true },
                new SystemSetting { SettingKey = "company.address", SettingValue = "123 Đường ABC, Q.1, TP.HCM", DataType = "string", GroupName = "contact", Description = "Địa chỉ công ty", IsVisible = true },

                // Service Fees
                new SystemSetting { SettingKey = "service.post_fee", SettingValue = "10000", DataType = "decimal", GroupName = "service", Description = "Phí đăng tin", IsVisible = true },
                new SystemSetting { SettingKey = "service.boost_fee", SettingValue = "50000", DataType = "decimal", GroupName = "service", Description = "Phí đẩy bài", IsVisible = true },
                new SystemSetting { SettingKey = "service.verify_fee", SettingValue = "100000", DataType = "decimal", GroupName = "service", Description = "Phí xác minh", IsVisible = true },

                // Policy
                new SystemSetting { SettingKey = "policy.auto_approve", SettingValue = "false", DataType = "boolean", GroupName = "policy", Description = "Tự động duyệt bài", IsVisible = true },
                new SystemSetting { SettingKey = "policy.review_timeout_hours", SettingValue = "24", DataType = "integer", GroupName = "policy", Description = "Thời gian duyệt tối đa (giờ)", IsVisible = true },

                // Security
                new SystemSetting { SettingKey = "security.require_email_verify", SettingValue = "true", DataType = "boolean", GroupName = "security", Description = "Yêu cầu xác minh email", IsVisible = true },
                new SystemSetting { SettingKey = "security.require_phone_verify", SettingValue = "false", DataType = "boolean", GroupName = "security", Description = "Yêu cầu xác minh điện thoại", IsVisible = true },
                new SystemSetting { SettingKey = "security.blocked_ips", SettingValue = "", DataType = "string", GroupName = "security", Description = "Danh sách IP bị chặn", IsVisible = true },

                // Appearance
                new SystemSetting { SettingKey = "appearance.theme_color", SettingValue = "blue", DataType = "string", GroupName = "appearance", Description = "Màu chủ đề", IsVisible = true },
                new SystemSetting { SettingKey = "appearance.logo_url", SettingValue = "/Content/img/logo.png", DataType = "string", GroupName = "appearance", Description = "URL logo", IsVisible = true },
                new SystemSetting { SettingKey = "appearance.language", SettingValue = "vi", DataType = "string", GroupName = "appearance", Description = "Ngôn ngữ mặc định", IsVisible = true },
            };

            context.SystemSettings.AddRange(defaultSettings);
            context.SaveChanges();
        }
    }
}
