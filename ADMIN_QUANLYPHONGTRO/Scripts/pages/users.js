/**
 * ========================================
 * USERS PAGE JAVASCRIPT
 * Quản lý người dùng - Admin Panel
 * ========================================
 */

var UsersPage = (function ($) {
    'use strict';

    // ============ PRIVATE VARIABLES ============
    var usersTable = null;
    var currentUserId = null;
    var config = {
        urls: {
            getUsers: '',
            getUserDetail: '',
            createUser: '',
            lockUser: '',
            unlockUser: ''
        }
    };

    // ============ INITIALIZATION ============
    function init(options) {
        config = $.extend(true, config, options);

        initDataTable();
        initFilterEvents();
        initModalEvents();

        console.log('✅ UsersPage initialized');
    }

    // ============ DATATABLE ============
    function initDataTable() {
        usersTable = $('#usersTable').DataTable({
            processing: true,
            serverSide: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            order: [[4, 'desc']], // Sắp xếp theo ngày tạo
            dom: '<"top"lf>rt<"bottom"ip><"clear">',
            ajax: {
                url: config.urls.getUsers,
                type: 'POST',
                dataType: 'json',
                data: function (d) {
                    // Thêm custom filter parameters
                    d.vaiTroId = $('#filterRole').val() || '';
                    
                    // Convert status filter: "active" -> không khóa, "locked" -> khóa
                    var statusVal = $('#filterStatus').val();
                    if (statusVal === 'active') {
                        d.status = 'active'; // Gửi đúng như frontend mong đợi
                    } else if (statusVal === 'locked') {
                        d.status = 'locked';
                    } else {
                        d.status = ''; // Không filter
                    }
                    
                    d.keyword = $('#searchInput').val() || '';
                    
                    console.log('📤 Sending filter:', d);
                },
                dataSrc: function (json) {
                    console.log('📦 DataTables Response:', json);
                    
                    // Kiểm tra response format
                    if (json.items && Array.isArray(json.items)) {
                        console.log('✅ Using backend format: items array');
                        return json.items;
                    } else if (json.data && Array.isArray(json.data)) {
                        console.log('✅ Using frontend format: data array');
                        return json.data;
                    } else {
                        console.warn('⚠️ Unexpected response format:', json);
                        return [];
                    }
                },
                error: function (xhr, error, thrown) {
                    console.error('❌ DataTable Error:', error, thrown);
                    try {
                        var response = JSON.parse(xhr.responseText);
                        console.error('Error Response:', response);
                    } catch (e) {
                        console.error('XHR Response:', xhr.responseText);
                    }
                    showNotification('error', 'Lỗi', 'Không thể tải dữ liệu người dùng');
                }
            },
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/vi.json',
                processing: '<div class="spinner-border text-primary" role="status"><span class="sr-only">Đang tải...</span></div>',
                emptyTable: 'Không có dữ liệu người dùng',
                zeroRecords: 'Không tìm thấy kết quả phù hợp'
            },
            columns: [
                { data: 'Avatar', name: 'avatar' },      // 0: Avatar
                { data: 'HoTen', name: 'info' },        // 1: Thông tin
                { data: 'DienThoai', name: 'phone' }, // 2: Điện thoại
                { data: 'VaiTroId', name: 'role' },  // 3: Vai trò
                { data: 'CreatedAt', name: 'created' }, // 4: Ngày tạo
                { data: 'IsKhoa', name: 'status' },  // 5: Trạng thái
                { data: 'NguoiDungId', name: 'actions' }      // 6: Thao tác
            ],
            columnDefs: [
                { orderable: false, targets: [0, 1, 6] },
                { className: 'text-center', targets: [0, 3, 5, 6] },
                {
                    targets: 0,
                    render: function (data, type, row) {
                        return renderAvatar(row);
                    }
                },
                {
                    targets: 1,
                    render: function (data, type, row) {
                        return renderUserInfo(row);
                    }
                },
                {
                    targets: 2,
                    render: function (data, type, row) {
                        return row.DienThoai || '<span class="text-muted">-</span>';
                    }
                },
                {
                    targets: 3,
                    render: function (data, type, row) {
                        return renderRole(row.VaiTroId, row.VaiTroName);
                    }
                },
                {
                    targets: 4,
                    render: function (data, type, row) {
                        return renderDate(row.CreatedAt);
                    }
                },
                {
                    targets: 5,
                    render: function (data, type, row) {
                        return renderStatus(row.IsKhoa);
                    }
                },
                {
                    targets: 6,
                    render: function (data, type, row) {
                        return renderActions(row);
                    }
                }
            ],
            drawCallback: function () {
                // Initialize tooltips safely if Popper.js is available
                if (typeof Popper !== 'undefined') {
                    try {
                        $('[data-toggle="tooltip"]').tooltip();
                    } catch (e) {
                        console.warn('⚠️ Tooltip error:', e.message);
                    }
                }
            }
        });
    }

    // ============ RENDER FUNCTIONS ============
    function renderAvatar(row) {
        var avatar = row.Avatar || '/Content/img/default-avatar.png';
        return '<img src="' + avatar + '" alt="Avatar" class="avatar-sm rounded-circle" ' +
               'onerror="this.onerror=null; this.src=\'/Content/img/default-avatar.png\';">';
    }

    function renderUserInfo(row) {
        var name = escapeHtml(row.HoTen || 'Chưa cập nhật');
        var email = escapeHtml(row.Email || '');
        
        return '<div class="user-info">' +
            '<strong>' + name + '</strong>' +
            '<br><small class="text-muted">' + email + '</small>' +
            '</div>';
    }

    function renderRole(roleId, roleName) {
        var badges = {
            1: { class: 'badge-danger', text: 'Admin', icon: 'fa-user-shield' },
            2: { class: 'badge-primary', text: 'Chủ trọ', icon: 'fa-home' },
            3: { class: 'badge-secondary', text: 'Người thuê', icon: 'fa-user' }
        };
        
        var badge = badges[roleId] || badges[3];
        return '<span class="badge ' + badge.class + '">' +
               '<i class="fa ' + badge.icon + ' mr-1"></i>' + (roleName || badge.text) +
               '</span>';
    }

    function renderDate(dateString) {
        if (!dateString) return '<span class="text-muted">-</span>';
        
        var date = new Date(dateString);
        if (isNaN(date.getTime())) return '<span class="text-muted">-</span>';
        
        var day = String(date.getDate()).padStart(2, '0');
        var month = String(date.getMonth() + 1).padStart(2, '0');
        var year = date.getFullYear();
        
        return day + '/' + month + '/' + year;
    }

    function renderStatus(isLocked) {
        if (isLocked) {
            return '<span class="badge badge-danger"><i class="fa fa-lock mr-1"></i>Đã khóa</span>';
        }
        return '<span class="badge badge-success"><i class="fa fa-check mr-1"></i>Hoạt động</span>';
    }

    function renderActions(row) {
        var userId = row.NguoiDungId;
        var isLocked = row.IsKhoa;
        
        var html = '<div class="btn-group" role="group">';
        
        // View button
        html += '<button type="button" class="btn btn-sm btn-info" ' +
                'onclick="UsersPage.viewDetail(\'' + userId + '\')" ' +
                'data-toggle="tooltip" title="Xem chi tiết">' +
                '<i class="fa fa-eye"></i></button>';
        
        // Lock/Unlock button
        if (isLocked) {
            html += '<button type="button" class="btn btn-sm btn-success" ' +
                    'onclick="UsersPage.unlockUser(\'' + userId + '\')" ' +
                    'data-toggle="tooltip" title="Mở khóa">' +
                    '<i class="fa fa-unlock"></i></button>';
        } else {
            html += '<button type="button" class="btn btn-sm btn-warning" ' +
                    'onclick="UsersPage.lockUser(\'' + userId + '\')" ' +
                    'data-toggle="tooltip" title="Khóa tài khoản">' +
                    '<i class="fa fa-lock"></i></button>';
        }
        
        html += '</div>';
        return html;
    }

    // ============ FILTER EVENTS ============
    function initFilterEvents() {
        // Nút tìm kiếm
        $('#btnSearch').on('click', function () {
            console.log('🔍 Search button clicked');
            if (usersTable) {
                usersTable.ajax.reload();
            }
        });

        // Tìm kiếm khi nhấn Enter
        $('#searchInput').on('keyup', function (e) {
            if (e.keyCode === 13) { // Enter key
                console.log('🔍 Enter pressed in search input');
                if (usersTable) {
                    usersTable.ajax.reload();
                }
            }
        });

        // Filter theo vai trò - reload tự động
        $('#filterRole').on('change', function () {
            console.log('🔍 Role filter changed to:', $(this).val());
            if (usersTable) {
                usersTable.ajax.reload();
            }
        });

        // Filter theo trạng thái - reload tự động
        $('#filterStatus').on('change', function () {
            console.log('🔍 Status filter changed to:', $(this).val());
            if (usersTable) {
                usersTable.ajax.reload();
            }
        });

        console.log('✅ Filter events initialized');
    }

    // ============ MODAL EVENTS ============
    function initModalEvents() {
        $('#addUserModal').on('hidden.bs.modal', function () {
            $('#addUserForm')[0].reset();
            selectRole(3);
        });
        
        $('#userDetailModal').on('hidden.bs.modal', function () {
            currentUserId = null;
        });
    }

    // ============ VIEW DETAIL ============
    function viewDetail(userId) {
        currentUserId = userId;
        
        $('#userDetailModal').modal('show');
        $('#userDetailLoading').show();
        $('#userDetailContent').hide();
        
        $.ajax({
            url: config.urls.getUserDetail,
            type: 'GET',
            data: { id: userId },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    populateUserDetail(response.data);
                    $('#userDetailLoading').hide();
                    $('#userDetailContent').show();
                } else {
                    showNotification('error', 'Lỗi', response.message || 'Không thể tải thông tin');
                    $('#userDetailModal').modal('hide');
                }
            },
            error: function () {
                showNotification('error', 'Lỗi', 'Không thể tải thông tin người dùng');
                $('#userDetailModal').modal('hide');
            }
        });
    }

    function populateUserDetail(user) {
        $('#detailAvatar').attr('src', user.Avatar || '/Content/img/default-avatar.png');
        $('#detailHoTen').text(user.HoTen || 'Chưa cập nhật');
        $('#detailEmail').text(user.Email || '-');
        $('#detailDienThoai').text(user.DienThoai || '-');
        $('#detailCCCD').text(user.LoaiGiayTo || '-');
        $('#detailNgayDangKy').text(renderDate(user.CreatedAt));
        $('#detailGhiChu').text(user.GhiChu || 'Không có ghi chú');
        
        // Role badge
        var roleBadge = renderRole(user.VaiTroId, user.VaiTroName);
        $('#detailVaiTro').html(roleBadge);
        
        // Status
        var statusHtml = user.IsKhoa 
            ? '<span class="badge badge-danger"><i class="fa fa-lock mr-1"></i>Đã khóa</span>'
            : '<span class="badge badge-success"><i class="fa fa-check mr-1"></i>Đang hoạt động</span>';
        $('#detailTrangThai').html(statusHtml);
        
        // Stats
        $('#detailSoPhong').text(user.SoPhongDaDang || 0);
        $('#detailSoDatPhong').text(user.SoDatPhong || 0);
    }

    // ============ LOCK/UNLOCK ============
    function lockUser(userId) {
        swal({
            title: 'Khóa tài khoản?',
            text: 'Người dùng sẽ không thể đăng nhập sau khi bị khóa.',
            icon: 'warning',
            buttons: {
                cancel: { text: 'Hủy', visible: true, className: 'btn btn-secondary' },
                confirm: { text: 'Khóa', className: 'btn btn-warning' }
            },
            dangerMode: true
        }).then(function (result) {
            if (result) {
                performLockUnlock(userId, config.urls.lockUser, 'khóa');
            }
        });
    }

    function unlockUser(userId) {
        swal({
            title: 'Mở khóa tài khoản?',
            text: 'Người dùng sẽ có thể đăng nhập lại.',
            icon: 'info',
            buttons: {
                cancel: { text: 'Hủy', visible: true, className: 'btn btn-secondary' },
                confirm: { text: 'Mở khóa', className: 'btn btn-success' }
            }
        }).then(function (result) {
            if (result) {
                performLockUnlock(userId, config.urls.unlockUser, 'mở khóa');
            }
        });
    }

    function performLockUnlock(userId, url, action) {
        $.ajax({
            url: url,
            type: 'POST',
            data: { id: userId },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    swal('Thành công!', 'Đã ' + action + ' tài khoản', 'success');
                    usersTable.ajax.reload(null, false);
                } else {
                    swal('Lỗi', response.message || 'Không thể ' + action, 'error');
                }
            },
            error: function () {
                swal('Lỗi', 'Có lỗi xảy ra khi ' + action + ' tài khoản', 'error');
            }
        });
    }

    // ============ HELPER FUNCTIONS ============
    function escapeHtml(text) {
        if (!text) return '';
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(text));
        return div.innerHTML;
    }

    function showNotification(type, title, message) {
        if (typeof swal !== 'undefined') {
            swal(title, message, type);
        } else {
            alert(title + ': ' + message);
        }
    }

    // ============ PUBLIC API ============
    return {
        init: init,
        viewDetail: viewDetail,
        lockUser: lockUser,
        unlockUser: unlockUser,
        reload: function () {
            if (usersTable) usersTable.ajax.reload();
        }
    };

})(jQuery);

// ============ GLOBAL FUNCTIONS ============
function initUsersPage(options) {
    UsersPage.init(options);
}

function selectRole(roleId) {
    $('.role-option').removeClass('active');
    $('.role-option[data-role="' + roleId + '"]').addClass('active');
    $('#addVaiTroId').val(roleId);
    
    // Show/hide extra info
    $('#landlordExtraInfo').toggle(roleId === 2);
    $('#adminExtraInfo').toggle(roleId === 1);
}

function togglePassword(inputId) {
    var input = document.getElementById(inputId);
    var icon = event.currentTarget.querySelector('i');
    
    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.replace('fa-eye', 'fa-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.replace('fa-eye-slash', 'fa-eye');
    }
}

function submitAddUser() {
    var form = $('#addUserForm');
    
    // Basic validation
    var email = $('#addEmail').val().trim();
    var password = $('#addPassword').val();
    var confirmPassword = $('#addConfirmPassword').val();
    
    if (!email) {
        $('#emailError').text('Vui lòng nhập email');
        return;
    }
    
    if (password.length < 6) {
        $('#passwordError').text('Mật khẩu tối thiểu 6 ký tự');
        return;
    }
    
    if (password !== confirmPassword) {
        $('#confirmPasswordError').text('Mật khẩu xác nhận không khớp');
        return;
    }
    
    // Clear errors
    $('.field-error').text('');
    
    // Submit
    var formData = {
        Email: email,
        Password: password,
        HoTen: $('#addHoTen').val().trim(),
        DienThoai: $('#addDienThoai').val().trim(),
        VaiTroId: parseInt($('#addVaiTroId').val()),
        IsEmailXacThuc: $('#addIsEmailXacThuc').is(':checked')
    };
    
    $('#btnSubmitAddUser').prop('disabled', true).html('<i class="fa fa-spinner fa-spin mr-1"></i> Đang xử lý...');
    
    $.ajax({
        url: UsersPage.urls ? UsersPage.urls.createUser : '/Users/CreateUser',
        type: 'POST',
        data: formData,
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                swal('Thành công!', 'Đã tạo người dùng mới', 'success');
                $('#addUserModal').modal('hide');
                UsersPage.reload();
            } else {
                swal('Lỗi', response.message || 'Không thể tạo người dùng', 'error');
            }
        },
        error: function () {
            swal('Lỗi', 'Có lỗi xảy ra khi tạo người dùng', 'error');
        },
        complete: function () {
            $('#btnSubmitAddUser').prop('disabled', false).html('<i class="fa fa-check mr-1"></i> Tạo người dùng');
        }
    });
}
