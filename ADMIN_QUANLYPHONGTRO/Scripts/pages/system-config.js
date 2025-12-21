/**
 * System Configuration Handler
 * Quản lý cấu hình hệ thống Admin
 * Gọi Controller thay vì API trực tiếp
 */

const SystemConfig = (function() {
    const controllerUrl = '/Settings';

    // Load all settings on page load
    const init = async () => {
        console.log('🔧 Initializing System Configuration');
        await loadAllSettings();
    };

    // Load all settings from Controller
    const loadAllSettings = async () => {
        try {
            console.log('📡 Loading all system settings...');
            const response = await fetch(`${controllerUrl}/GetAllSystemSettings`);
            const result = await response.json();

            console.log('📋 API Response:', result);

            if (result.success && result.data) {
                console.log(`✅ Loaded ${result.data.length} settings`);
                populateSettings(result.data);
            } else {
                console.warn('⚠️ No success in response or no data');
            }
        } catch (error) {
            console.error('❌ Error loading settings:', error);
            toastr.error('Lỗi tải cài đặt: ' + error.message, 'Cảnh báo');
        }
    };

    // Populate form fields from settings data
    const populateSettings = (settings) => {
        console.log('🔍 Populating settings...');
        const settingsMap = {};
        
        settings.forEach(s => {
            // Handle both camelCase and PascalCase property names
            const key = s.settingKey || s.SettingKey;
            const value = s.settingValue || s.SettingValue;
            
            console.log(`  ${key} = ${value}`);
            settingsMap[key] = value;
        });

        // General Info
        $('#app_name').val(settingsMap['app.name'] || '');
        $('#app_description').val(settingsMap['app.description'] || '');
        $('#app_url').val(settingsMap['app.url'] || '');

        // Contact Info
        $('#support_hotline').val(settingsMap['support.hotline'] || '');
        $('#support_email').val(settingsMap['support.email'] || '');
        $('#company_address').val(settingsMap['company.address'] || '');

        // Service Fees
        $('#service_post_fee').val(settingsMap['service.post_fee'] || '0');
        $('#service_boost_fee').val(settingsMap['service.boost_fee'] || '0');
        $('#service_verify_fee').val(settingsMap['service.verify_fee'] || '0');

        // Policies
        $('#auto_approve').prop('checked', settingsMap['policy.auto_approve'] === 'true');
        $('#review_timeout_hours').val(settingsMap['policy.review_timeout_hours'] || '24');

        // Security
        $('#require_email_verify').prop('checked', settingsMap['security.require_email_verify'] === 'true');
        $('#require_phone_verify').prop('checked', settingsMap['security.require_phone_verify'] === 'true');
        $('#blocked_ips').val(settingsMap['security.blocked_ips'] || '');

        // Appearance
        $('#theme_color').val(settingsMap['appearance.theme_color'] || 'blue');
        $('#logo_url').val(settingsMap['appearance.logo_url'] || '');
        $('#default_language').val(settingsMap['appearance.language'] || 'vi');
        
        console.log('✅ Settings populated successfully');
    };

    // Save General Settings
    const saveGeneral = async () => {
        const settings = {
            'app.name': $('#app_name').val(),
            'app.description': $('#app_description').val(),
            'app.url': $('#app_url').val()
        };

        await saveBatchSettings(settings, 'Thông tin chung');
    };

    // Save Contact Settings
    const saveContact = async () => {
        const settings = {
            'support.hotline': $('#support_hotline').val(),
            'support.email': $('#support_email').val(),
            'company.address': $('#company_address').val()
        };

        await saveBatchSettings(settings, 'Thông tin liên hệ');
    };

    // Save Service Fees
    const saveService = async () => {
        const settings = {
            'service.post_fee': $('#service_post_fee').val(),
            'service.boost_fee': $('#service_boost_fee').val(),
            'service.verify_fee': $('#service_verify_fee').val()
        };

        await saveBatchSettings(settings, 'Phí dịch vụ');
    };

    // Save Policies
    const savePolicies = async () => {
        const settings = {
            'policy.auto_approve': $('#auto_approve').is(':checked') ? 'true' : 'false',
            'policy.review_timeout_hours': $('#review_timeout_hours').val()
        };

        await saveBatchSettings(settings, 'Chính sách duyệt bài');
    };

    // Save Security Settings
    const saveSecurity = async () => {
        const settings = {
            'security.require_email_verify': $('#require_email_verify').is(':checked') ? 'true' : 'false',
            'security.require_phone_verify': $('#require_phone_verify').is(':checked') ? 'true' : 'false',
            'security.blocked_ips': $('#blocked_ips').val()
        };

        await saveBatchSettings(settings, 'Bảo mật');
    };

    // Save Appearance Settings
    const saveAppearance = async () => {
        const settings = {
            'appearance.theme_color': $('#theme_color').val(),
            'appearance.logo_url': $('#logo_url').val(),
            'appearance.language': $('#default_language').val()
        };

        await saveBatchSettings(settings, 'Giao diện');
    };

    // Batch update settings via Controller
    const saveBatchSettings = async (settings, type) => {
        try {
            // Find and get button element
            const btn = event?.target;
            if (!btn) {
                toastr.error('Không tìm thấy button', 'Lỗi');
                return;
            }

            const originalText = btn.innerHTML;
            btn.disabled = true;
            btn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Đang lưu...';

            console.log(`💾 Saving ${type} settings...`);

            const response = await fetch(`${controllerUrl}/UpdateSystemSettingsByKey`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(settings)
            });

            const result = await response.json();

            btn.disabled = false;
            btn.innerHTML = originalText;

            if (result.success) {
                console.log(`✅ ${type} saved successfully`);
                toastr.success(`Lưu ${type} thành công!`, 'Thành công');
            } else {
                console.error(`❌ Failed to save ${type}:`, result.message);
                toastr.error(result.message || `Lỗi lưu ${type}`, 'Lỗi');
            }
        } catch (error) {
            console.error(`❌ Error saving ${type} settings:`, error);
            toastr.error(`Lỗi lưu ${type}: ${error.message}`, 'Cảnh báo');

            // Restore button
            if (event?.target) {
                event.target.disabled = false;
                event.target.innerHTML = '<i class="fa fa-save mr-1"></i> Lưu';
            }
        }
    };

    // Public API
    return {
        init,
        saveGeneral,
        saveContact,
        saveService,
        savePolicies,
        saveSecurity,
        saveAppearance
    };
})();

// Initialize on page load
$(document).ready(() => {
    SystemConfig.init();
});
