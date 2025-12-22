/**
 * ========================================
 * SETTINGS PAGE JAVASCRIPT
 * Quản lý cài đặt hệ thống - Admin Panel
 * ========================================
 */

// Namespace để tránh xung đột
var SettingsPage = (function ($) {
    'use strict';

    // ============ PRIVATE VARIABLES ============
    var tienIchData = [];
    var quanHuyenData = [];
    var phuongData = [];

    // API URLs - sẽ được set từ view
    var apiUrls = {
        // Tiện ích
        getTienIch: '',
        createTienIch: '',
        updateTienIch: '',
        deleteTienIch: '',
        // Quận/Huyện
        getQuanHuyen: '',
        createQuanHuyen: '',
        updateQuanHuyen: '',
        deleteQuanHuyen: '',
        // Phường
        getPhuong: '',
        createPhuong: '',
        updatePhuong: '',
        deletePhuong: ''
    };

    // ============ INITIALIZATION ============
    function init(urls) {
        apiUrls = $.extend(apiUrls, urls);

        // Load data khi tab được chọn
        initTabEvents();

        // Load dữ liệu ban đầu cho tab Tiện ích (đang active)
        loadTienIch();

        console.log('✅ SettingsPage initialized');
    }

    function initTabEvents() {
        // Khi chuyển tab, load data tương ứng
        $('#settingsTabs a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
            var target = $(e.target).attr('href');
            
            if (target === '#tienich-content') {
                loadTienIch();
            } else if (target === '#quanhuyen-content') {
                loadQuanHuyen();
            } else if (target === '#phuong-content') {
                loadQuanHuyenForDropdown();
                loadPhuong();
            }
        });

        // Filter phường theo quận
        $('#filterQuanHuyen').on('change', function () {
            loadPhuong($(this).val());
        });
    }

    // ============ TIỆN ÍCH ============
    function loadTienIch() {
        showLoading('tienIch');

        $.ajax({
            url: apiUrls.getTienIch,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                hideLoading('tienIch');

                if (response.success) {
                    tienIchData = response.data || [];
                    renderTienIchTable();
                } else {
                    showError('tienIch', response.message || 'Không thể tải dữ liệu');
                }
            },
            error: function (xhr, error) {
                hideLoading('tienIch');
                console.error('❌ Load TienIch error:', error);
                showError('tienIch', 'Có lỗi xảy ra khi tải dữ liệu');
            }
        });
    }

    function renderTienIchTable() {
        var $tbody = $('#tienIchTableBody');
        $tbody.empty();

        if (tienIchData.length === 0) {
            $('#tienIchEmpty').show();
            return;
        }

        $('#tienIchEmpty').hide();

        tienIchData.forEach(function (item, index) {
            var row = '<tr>' +
                '<td>' + (index + 1) + '</td>' +
                '<td>' + escapeHtml(item.ten) + '</td>' +
                '<td class="text-center">' +
                    '<button type="button" class="btn btn-sm btn-warning mr-1" onclick="SettingsPage.editTienIch(' + item.tienIchId + ')" title="Sửa">' +
                        '<i class="fa fa-edit"></i>' +
                    '</button>' +
                    '<button type="button" class="btn btn-sm btn-danger" onclick="SettingsPage.deleteTienIch(' + item.tienIchId + ', \'' + escapeHtml(item.ten) + '\')" title="Xóa">' +
                        '<i class="fa fa-trash"></i>' +
                    '</button>' +
                '</td>' +
                '</tr>';
            $tbody.append(row);
        });
    }

    function showAddTienIch() {
        $('#tienIchId').val('0');
        $('#tienIchTen').val('');
        $('#tienIchModalTitle').html('<i class="fa fa-wifi mr-2"></i>Thêm tiện ích');
        $('#tienIchModal').modal('show');
        setTimeout(function () { $('#tienIchTen').focus(); }, 300);
    }

    function editTienIch(id) {
        var item = tienIchData.find(function (x) { return x.tienIchId === id; });
        if (!item) {
            showNotification('error', 'Lỗi', 'Không tìm thấy tiện ích');
            return;
        }

        $('#tienIchId').val(item.tienIchId);
        $('#tienIchTen').val(item.ten);
        $('#tienIchModalTitle').html('<i class="fa fa-edit mr-2"></i>Sửa tiện ích');
        $('#tienIchModal').modal('show');
        setTimeout(function () { $('#tienIchTen').focus(); }, 300);
    }

    function saveTienIch() {
        var id = parseInt($('#tienIchId').val()) || 0;
        var ten = $('#tienIchTen').val().trim();

        if (!ten) {
            showNotification('warning', 'Cảnh báo', 'Vui lòng nhập tên tiện ích');
            $('#tienIchTen').focus();
            return;
        }

        var url = id > 0 ? apiUrls.updateTienIch : apiUrls.createTienIch;
        var data = id > 0 ? { id: id, ten: ten } : { ten: ten };

        showProcessing();

        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    swal({
                        title: 'Thành công!',
                        text: response.message || (id > 0 ? 'Cập nhật thành công' : 'Thêm mới thành công'),
                        icon: 'success',
                        button: 'OK'
                    }).then(function () {
                        $('#tienIchModal').modal('hide');
                        loadTienIch();
                    });
                } else {
                    swal('Lỗi', response.message || 'Không thể lưu dữ liệu', 'error');
                }
            },
            error: function (xhr, error) {
                console.error('❌ Save TienIch error:', error);
                swal('Lỗi', 'Có lỗi xảy ra khi lưu dữ liệu', 'error');
            }
        });
    }

    function deleteTienIch(id, ten) {
        swal({
            title: 'Xác nhận xóa?',
            text: 'Bạn có chắc muốn xóa tiện ích "' + ten + '"?',
            icon: 'warning',
            buttons: {
                cancel: { text: 'Hủy', visible: true, className: 'btn btn-secondary' },
                confirm: { text: 'Xóa', className: 'btn btn-danger' }
            },
            dangerMode: true
        }).then(function (result) {
            if (result) {
                performDelete(apiUrls.deleteTienIch, { id: id }, 'tiện ích', loadTienIch);
            }
        });
    }

    // ============ QUẬN/HUYỆN ============
    function loadQuanHuyen() {
        showLoading('quanHuyen');

        $.ajax({
            url: apiUrls.getQuanHuyen,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                hideLoading('quanHuyen');

                if (response.success) {
                    quanHuyenData = response.data || [];
                    renderQuanHuyenTable();
                } else {
                    showError('quanHuyen', response.message || 'Không thể tải dữ liệu');
                }
            },
            error: function (xhr, error) {
                hideLoading('quanHuyen');
                console.error('❌ Load QuanHuyen error:', error);
                showError('quanHuyen', 'Có lỗi xảy ra khi tải dữ liệu');
            }
        });
    }

    function renderQuanHuyenTable() {
        var $tbody = $('#quanHuyenTableBody');
        $tbody.empty();

        if (quanHuyenData.length === 0) {
            $('#quanHuyenEmpty').show();
            return;
        }

        $('#quanHuyenEmpty').hide();

        quanHuyenData.forEach(function (item, index) {
            var row = '<tr>' +
                '<td>' + (index + 1) + '</td>' +
                '<td>' + escapeHtml(item.ten) + '</td>' +
                '<td class="text-center">' +
                    '<button type="button" class="btn btn-sm btn-warning mr-1" onclick="SettingsPage.editQuanHuyen(' + item.quanHuyenId + ')" title="Sửa">' +
                        '<i class="fa fa-edit"></i>' +
                    '</button>' +
                    '<button type="button" class="btn btn-sm btn-danger" onclick="SettingsPage.deleteQuanHuyen(' + item.quanHuyenId + ', \'' + escapeHtml(item.ten) + '\')" title="Xóa">' +
                        '<i class="fa fa-trash"></i>' +
                    '</button>' +
                '</td>' +
                '</tr>';
            $tbody.append(row);
        });
    }

    function loadQuanHuyenForDropdown() {
        $.ajax({
            url: apiUrls.getQuanHuyen,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    quanHuyenData = response.data || [];
                    
                    // Update filter dropdown
                    var $filter = $('#filterQuanHuyen');
                    $filter.find('option:not(:first)').remove();
                    quanHuyenData.forEach(function (item) {
                        $filter.append('<option value="' + item.quanHuyenId + '">' + escapeHtml(item.ten) + '</option>');
                    });

                    // Update modal dropdown
                    var $modal = $('#phuongQuanHuyenId');
                    $modal.find('option:not(:first)').remove();
                    quanHuyenData.forEach(function (item) {
                        $modal.append('<option value="' + item.quanHuyenId + '">' + escapeHtml(item.ten) + '</option>');
                    });
                }
            },
            error: function (xhr, error) {
                console.error('❌ Load QuanHuyen for dropdown error:', error);
            }
        });
    }

    function showAddQuanHuyen() {
        $('#quanHuyenId').val('0');
        $('#quanHuyenTen').val('');
        $('#quanHuyenModalTitle').html('<i class="fa fa-map-marker-alt mr-2"></i>Thêm quận/huyện');
        $('#quanHuyenModal').modal('show');
        setTimeout(function () { $('#quanHuyenTen').focus(); }, 300);
    }

    function editQuanHuyen(id) {
        var item = quanHuyenData.find(function (x) { return x.quanHuyenId === id; });
        if (!item) {
            showNotification('error', 'Lỗi', 'Không tìm thấy quận/huyện');
            return;
        }

        $('#quanHuyenId').val(item.quanHuyenId);
        $('#quanHuyenTen').val(item.ten);
        $('#quanHuyenModalTitle').html('<i class="fa fa-edit mr-2"></i>Sửa quận/huyện');
        $('#quanHuyenModal').modal('show');
        setTimeout(function () { $('#quanHuyenTen').focus(); }, 300);
    }

    function saveQuanHuyen() {
        var id = parseInt($('#quanHuyenId').val()) || 0;
        var ten = $('#quanHuyenTen').val().trim();

        if (!ten) {
            showNotification('warning', 'Cảnh báo', 'Vui lòng nhập tên quận/huyện');
            $('#quanHuyenTen').focus();
            return;
        }

        var url = id > 0 ? apiUrls.updateQuanHuyen : apiUrls.createQuanHuyen;
        var data = id > 0 ? { id: id, ten: ten } : { ten: ten };

        showProcessing();

        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    swal({
                        title: 'Thành công!',
                        text: response.message || (id > 0 ? 'Cập nhật thành công' : 'Thêm mới thành công'),
                        icon: 'success',
                        button: 'OK'
                    }).then(function () {
                        $('#quanHuyenModal').modal('hide');
                        loadQuanHuyen();
                        loadQuanHuyenForDropdown();
                    });
                } else {
                    swal('Lỗi', response.message || 'Không thể lưu dữ liệu', 'error');
                }
            },
            error: function (xhr, error) {
                console.error('❌ Save QuanHuyen error:', error);
                swal('Lỗi', 'Có lỗi xảy ra khi lưu dữ liệu', 'error');
            }
        });
    }

    function deleteQuanHuyen(id, ten) {
        swal({
            title: 'Xác nhận xóa?',
            text: 'Bạn có chắc muốn xóa quận/huyện "' + ten + '"?\n\nLưu ý: Không thể xóa nếu đang có nhà trọ thuộc quận/huyện này.',
            icon: 'warning',
            buttons: {
                cancel: { text: 'Hủy', visible: true, className: 'btn btn-secondary' },
                confirm: { text: 'Xóa', className: 'btn btn-danger' }
            },
            dangerMode: true
        }).then(function (result) {
            if (result) {
                performDelete(apiUrls.deleteQuanHuyen, { id: id }, 'quận/huyện', function () {
                    loadQuanHuyen();
                    loadQuanHuyenForDropdown();
                });
            }
        });
    }

    // ============ PHƯỜNG ============
    function loadPhuong(quanHuyenId) {
        showLoading('phuong');

        var url = apiUrls.getPhuong;
        if (quanHuyenId) {
            url += '?quanHuyenId=' + quanHuyenId;
        }

        $.ajax({
            url: url,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                hideLoading('phuong');

                if (response.success) {
                    phuongData = response.data || [];
                    renderPhuongTable();
                } else {
                    showError('phuong', response.message || 'Không thể tải dữ liệu');
                }
            },
            error: function (xhr, error) {
                hideLoading('phuong');
                console.error('❌ Load Phuong error:', error);
                showError('phuong', 'Có lỗi xảy ra khi tải dữ liệu');
            }
        });
    }

    function renderPhuongTable() {
        var $tbody = $('#phuongTableBody');
        $tbody.empty();

        if (phuongData.length === 0) {
            $('#phuongEmpty').show();
            return;
        }

        $('#phuongEmpty').hide();

        phuongData.forEach(function (item, index) {
            // Tìm tên quận
            var quanHuyen = quanHuyenData.find(function (q) { return q.quanHuyenId === item.quanHuyenId; });
            var quanHuyenTen = quanHuyen ? quanHuyen.ten : (item.quanHuyenTen || 'Không xác định');

            var row = '<tr>' +
                '<td>' + (index + 1) + '</td>' +
                '<td>' + escapeHtml(item.ten) + '</td>' +
                '<td>' + escapeHtml(quanHuyenTen) + '</td>' +
                '<td class="text-center">' +
                    '<button type="button" class="btn btn-sm btn-warning mr-1" onclick="SettingsPage.editPhuong(' + item.phuongId + ')" title="Sửa">' +
                        '<i class="fa fa-edit"></i>' +
                    '</button>' +
                    '<button type="button" class="btn btn-sm btn-danger" onclick="SettingsPage.deletePhuong(' + item.phuongId + ', \'' + escapeHtml(item.ten) + '\')" title="Xóa">' +
                        '<i class="fa fa-trash"></i>' +
                    '</button>' +
                '</td>' +
                '</tr>';
            $tbody.append(row);
        });
    }

    function showAddPhuong() {
        $('#phuongId').val('0');
        $('#phuongQuanHuyenId').val('');
        $('#phuongTen').val('');
        $('#phuongModalTitle').html('<i class="fa fa-building mr-2"></i>Thêm phường/xã');
        $('#phuongModal').modal('show');
        setTimeout(function () { $('#phuongQuanHuyenId').focus(); }, 300);
    }

    function editPhuong(id) {
        var item = phuongData.find(function (x) { return x.phuongId === id; });
        if (!item) {
            showNotification('error', 'Lỗi', 'Không tìm thấy phường/xã');
            return;
        }

        $('#phuongId').val(item.phuongId);
        $('#phuongQuanHuyenId').val(item.quanHuyenId);
        $('#phuongTen').val(item.ten);
        $('#phuongModalTitle').html('<i class="fa fa-edit mr-2"></i>Sửa phường/xã');
        $('#phuongModal').modal('show');
        setTimeout(function () { $('#phuongTen').focus(); }, 300);
    }

    function savePhuong() {
        var id = parseInt($('#phuongId').val()) || 0;
        var quanHuyenId = parseInt($('#phuongQuanHuyenId').val()) || 0;
        var ten = $('#phuongTen').val().trim();

        if (!quanHuyenId) {
            showNotification('warning', 'Cảnh báo', 'Vui lòng chọn quận/huyện');
            $('#phuongQuanHuyenId').focus();
            return;
        }

        if (!ten) {
            showNotification('warning', 'Cảnh báo', 'Vui lòng nhập tên phường/xã');
            $('#phuongTen').focus();
            return;
        }

        var url = id > 0 ? apiUrls.updatePhuong : apiUrls.createPhuong;
        var data = id > 0 
            ? { id: id, quanHuyenId: quanHuyenId, ten: ten } 
            : { quanHuyenId: quanHuyenId, ten: ten };

        showProcessing();

        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    swal({
                        title: 'Thành công!',
                        text: response.message || (id > 0 ? 'Cập nhật thành công' : 'Thêm mới thành công'),
                        icon: 'success',
                        button: 'OK'
                    }).then(function () {
                        $('#phuongModal').modal('hide');
                        loadPhuong($('#filterQuanHuyen').val());
                    });
                } else {
                    swal('Lỗi', response.message || 'Không thể lưu dữ liệu', 'error');
                }
            },
            error: function (xhr, error) {
                console.error('❌ Save Phuong error:', error);
                swal('Lỗi', 'Có lỗi xảy ra khi lưu dữ liệu', 'error');
            }
        });
    }

    function deletePhuong(id, ten) {
        swal({
            title: 'Xác nhận xóa?',
            text: 'Bạn có chắc muốn xóa phường/xã "' + ten + '"?',
            icon: 'warning',
            buttons: {
                cancel: { text: 'Hủy', visible: true, className: 'btn btn-secondary' },
                confirm: { text: 'Xóa', className: 'btn btn-danger' }
            },
            dangerMode: true
        }).then(function (result) {
            if (result) {
                performDelete(apiUrls.deletePhuong, { id: id }, 'phường/xã', function () {
                    loadPhuong($('#filterQuanHuyen').val());
                });
            }
        });
    }

    // ============ HELPER FUNCTIONS ============
    function showLoading(type) {
        $('#' + type + 'TableBody').hide();
        $('#' + type + 'Empty').hide();
        $('#' + type + 'Loading').show();
    }

    function hideLoading(type) {
        $('#' + type + 'Loading').hide();
        $('#' + type + 'TableBody').show();
    }

    function showError(type, message) {
        $('#' + type + 'TableBody').hide();
        $('#' + type + 'Empty').show().html(
            '<i class="fa fa-exclamation-circle fa-3x text-danger mb-3"></i>' +
            '<p class="text-danger">' + escapeHtml(message) + '</p>'
        );
    }

    function showProcessing() {
        swal({
            title: 'Đang xử lý...',
            text: 'Vui lòng chờ',
            icon: 'info',
            buttons: false,
            closeOnClickOutside: false,
            closeOnEsc: false
        });
    }

    function performDelete(url, data, itemName, callback) {
        showProcessing();

        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    swal({
                        title: 'Thành công!',
                        text: response.message || 'Đã xóa ' + itemName,
                        icon: 'success',
                        button: 'OK'
                    }).then(function () {
                        if (typeof callback === 'function') {
                            callback();
                        }
                    });
                } else {
                    swal('Lỗi', response.message || 'Không thể xóa ' + itemName, 'error');
                }
            },
            error: function (xhr, error) {
                console.error('❌ Delete error:', error);
                swal('Lỗi', 'Có lỗi xảy ra khi xóa ' + itemName, 'error');
            }
        });
    }

    function showNotification(type, title, message) {
        if (typeof swal !== 'undefined') {
            swal(title, message, type);
        } else {
            alert(title + ': ' + message);
        }
    }

    function escapeHtml(text) {
        if (!text) return '';
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(text));
        return div.innerHTML;
    }

    // ============ PUBLIC API ============
    return {
        init: init,
        // Tiện ích
        showAddTienIch: showAddTienIch,
        editTienIch: editTienIch,
        saveTienIch: saveTienIch,
        deleteTienIch: deleteTienIch,
        // Quận/Huyện
        showAddQuanHuyen: showAddQuanHuyen,
        editQuanHuyen: editQuanHuyen,
        saveQuanHuyen: saveQuanHuyen,
        deleteQuanHuyen: deleteQuanHuyen,
        // Phường
        showAddPhuong: showAddPhuong,
        editPhuong: editPhuong,
        savePhuong: savePhuong,
        deletePhuong: deletePhuong
    };

})(jQuery);
