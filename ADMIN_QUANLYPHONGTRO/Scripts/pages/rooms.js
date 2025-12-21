/**
 * ========================================
 * ROOMS PAGE JAVASCRIPT
 * Quản lý phòng trọ - Admin Panel
 * ========================================
 */

// Namespace để tránh xung đột
var RoomsPage = (function ($) {
    'use strict';

    // ============ PRIVATE VARIABLES ============
    var roomsTable = null;
    var currentRoomId = null;
    var currentFilter = {
        status: '',
        keyword: ''
    };

    // API URLs - sẽ được set từ view
    var apiUrls = {
        getPendingRooms: '',
        getAllRooms: '',
        getRoomDetail: '',
        approveRoom: '',
        rejectRoom: '',
        toggleLockRoom: '',
        getRoomStats: ''
    };

    // ============ INITIALIZATION ============
    function init(urls) {
        apiUrls = $.extend(apiUrls, urls);

        initDataTable();
        initFilterEvents();
        initModalEvents();
        loadStats();

        console.log('✅ RoomsPage initialized');
    }

    // ============ DATATABLE ============
    function initDataTable() {
        var ajaxUrl = apiUrls.getPendingRooms || apiUrls.getAllRooms;

        roomsTable = $('#roomsTable').DataTable({
            processing: true,
            serverSide: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            order: [[5, 'desc']], // Sắp xếp theo ngày tạo
            dom: '<"top"l>rt<"bottom"ip><"clear">',
            ajax: {
                url: ajaxUrl,
                type: 'POST',
                dataType: 'json',
                data: function (d) {
                    // Thêm filter parameters
                    d.status = $('#filterStatus').val();
                    d.search = $('#filterKeyword').val().trim();
                    console.log('📤 Sending DataTables request:', {
                        draw: d.draw,
                        start: d.start,
                        length: d.length,
                        status: d.status,
                        search: d.search
                    });
                },
                dataSrc: function (json) {
                    console.log('📦 DataTables Response:', json);
                    return json.data || [];
                },
                error: function (xhr, error, thrown) {
                    console.error('❌ DataTable Error:', error, thrown);
                    console.error('Response:', xhr.responseText);
                    showNotification('error', 'Lỗi', 'Không thể tải dữ liệu. Vui lòng thử lại.');
                }
            },
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/vi.json',
                processing: '<div class="loading-spinner"></div>',
                emptyTable: 'Không có dữ liệu',
                zeroRecords: 'Không tìm thấy kết quả phù hợp'
            },
            columns: [
                { data: null, name: 'stt' },            // 0: STT
                { data: null, name: 'room' },           // 1: Phòng
                { data: 'DienTich', name: 'area' },     // 2: Diện tích
                { data: 'GiaTien', name: 'price' },     // 3: Giá tiền
                { data: 'ChuTroName', name: 'host' },   // 4: Chủ trọ
                { data: 'CreatedAt', name: 'date' },    // 5: Ngày tạo
                { data: null, name: 'status' },         // 6: Trạng thái
                { data: null, name: 'actions' }         // 7: Hành động
            ],
            columnDefs: [
                { orderable: false, targets: [0, 1, 7] },
                { className: 'text-center', targets: [7] },
                {
                    targets: 0,
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    targets: 1,
                    render: function (data, type, row) {
                        return renderRoomInfo(row);
                    }
                },
                {
                    targets: 2,
                    render: function (data, type, row) {
                        return renderArea(row.DienTich);
                    }
                },
                {
                    targets: 3,
                    render: function (data, type, row) {
                        return renderPrice(row.GiaTien, row.TienCoc);
                    }
                },
                {
                    targets: 4,
                    render: function (data, type, row) {
                        return renderHost(row.ChuTroName);
                    }
                },
                {
                    targets: 5,
                    render: function (data, type, row) {
                        return renderDate(row.CreatedAt);
                    }
                },
                {
                    targets: 6,
                    render: function (data, type, row) {
                        return renderStatus(row);
                    }
                },
                {
                    targets: 7,
                    render: function (data, type, row) {
                        return renderActions(row);
                    }
                }
            ],
            drawCallback: function () {
                // Reinitialize tooltips after each draw
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
    function renderRoomInfo(row) {
        var image = row.ImageUrl || '/Content/img/no-image.png';
        var title = escapeHtml(row.TieuDe || 'Chưa có tiêu đề');
        var location = escapeHtml(row.NhaTroName || 'Chưa có thông tin');

        return '<div class="room-info">' +
            '<img src="' + image + '" alt="' + title + '" class="room-image" ' +
            'onerror="this.onerror=null; this.src=\'/Content/img/no-image.png\';" />' +
            '<div class="room-details">' +
            '<div class="room-title" title="' + title + '">' + title + '</div>' +
            '<div class="room-location" title="' + location + '">' + location + '</div>' +
            '</div>' +
            '</div>';
    }

    function renderArea(area) {
        if (!area || area <= 0) {
            return '<span class="area-cell text-muted">-</span>';
        }
        return '<span class="area-cell">' + area + ' m²</span>';
    }

    function renderPrice(price, deposit) {
        var priceText = formatCurrency(price || 0);
        var depositText = deposit ? 'Cọc: ' + formatCurrency(deposit) : '';

        return '<div class="price-cell">' +
            priceText +
            (depositText ? '<div class="deposit">' + depositText + '</div>' : '') +
            '</div>';
    }

    function renderHost(hostName) {
        if (!hostName) {
            return '<span class="host-cell text-muted">Chưa có thông tin</span>';
        }
        return '<span class="host-cell">' + escapeHtml(hostName) + '</span>';
    }

    function renderDate(dateString) {
        if (!dateString) return '<span class="text-muted">-</span>';

        var date = new Date(dateString);
        if (isNaN(date.getTime())) return '<span class="text-muted">-</span>';

        var formattedDate = formatDate(date);
        var relativeTime = getRelativeTime(date);

        return '<div class="date-cell">' +
            formattedDate +
            '<div class="time">' + relativeTime + '</div>' +
            '</div>';
    }

    function renderStatus(row) {
        var statusClass = 'pending';
        var statusIcon = 'fa-clock';
        var statusText = 'Chờ duyệt';

        if (row.IsBiKhoa) {
            statusClass = 'locked';
            statusIcon = 'fa-lock';
            statusText = 'Bị khóa';
        } else if (row.IsDuyet) {
            statusClass = 'approved';
            statusIcon = 'fa-check-circle';
            statusText = 'Đã duyệt';
        }

        return '<span class="status-badge ' + statusClass + '">' +
            '<i class="fa ' + statusIcon + '"></i> ' + statusText +
            '</span>';
    }

    function renderActions(row) {
        var roomId = row.PhongId;
        var isPending = !row.IsDuyet && !row.IsBiKhoa;
        var isLocked = row.IsBiKhoa;

        var html = '<div class="action-buttons">';

        // Nút xem chi tiết - luôn hiển thị
        html += '<button type="button" class="btn-action btn-view" ' +
            'onclick="RoomsPage.viewDetail(\'' + roomId + '\')" ' +
            'data-toggle="tooltip" data-placement="top" title="Xem chi tiết">' +
            '<i class="fa fa-eye"></i>' +
            '</button>';

        // Nếu đang chờ duyệt, hiển thị nút duyệt/từ chối
        if (isPending) {
            html += '<button type="button" class="btn-action btn-approve" ' +
                'onclick="RoomsPage.quickApprove(\'' + roomId + '\')" ' +
                'data-toggle="tooltip" data-placement="top" title="Duyệt nhanh">' +
                '<i class="fa fa-check"></i>' +
                '</button>';

            html += '<button type="button" class="btn-action btn-reject" ' +
                'onclick="RoomsPage.quickReject(\'' + roomId + '\')" ' +
                'data-toggle="tooltip" data-placement="top" title="Từ chối">' +
                '<i class="fa fa-times"></i>' +
                '</button>';
        }

        // Nút khóa/mở khóa
        if (isLocked) {
            html += '<button type="button" class="btn-action btn-lock" ' +
                'onclick="RoomsPage.toggleLock(\'' + roomId + '\', false)" ' +
                'data-toggle="tooltip" data-placement="top" title="Mở khóa">' +
                '<i class="fa fa-unlock"></i>' +
                '</button>';
        } else {
            html += '<button type="button" class="btn-action btn-lock" ' +
                'onclick="RoomsPage.toggleLock(\'' + roomId + '\', true)" ' +
                'data-toggle="tooltip" data-placement="top" title="Khóa phòng">' +
                '<i class="fa fa-lock"></i>' +
                '</button>';
        }

        html += '</div>';
        return html;
    }

    // ============ FILTER EVENTS ============
    function initFilterEvents() {
        // Search on enter
        $('#filterKeyword').on('keyup', function (e) {
            if (e.keyCode === 13) {
                applyFilter();
            }
        });

        // Search button click
        $('#btnApplyFilter').on('click', function () {
            applyFilter();
        });

        // Status filter change - reload tự động
        $('#filterStatus').on('change', function () {
            applyFilter();
        });

        console.log('✅ Filter events initialized');
    }

    function applyFilter() {
        console.log('🔍 Applying filter with:', {
            status: $('#filterStatus').val(),
            keyword: $('#filterKeyword').val().trim()
        });
        
        if (roomsTable) {
            roomsTable.ajax.reload(null, false);
        }
    }

    function refreshTable() {
        console.log('🔄 Refreshing table...');
        // Reset filters
        $('#filterStatus').val('pending');
        $('#filterKeyword').val('');
        
        if (roomsTable) {
            roomsTable.ajax.reload(null, true); // true = reset to page 1
        }
        
        loadStats();
    }

    // ============ MODAL EVENTS ============
    function initModalEvents() {
        // Modal close - reset currentRoomId
        $('#roomDetailModal').on('hidden.bs.modal', function () {
            currentRoomId = null;
            $('#rejectReason').val('');
        });
    }

    // ============ VIEW DETAIL ============
    function viewDetail(roomId) {
        currentRoomId = roomId;

        // Show modal with loading state
        $('#roomDetailModal').modal('show');
        $('#roomDetailLoading').show();
        $('#roomDetailContent').hide();

        // Load room detail
        $.ajax({
            url: apiUrls.getRoomDetail,
            type: 'GET',
            data: { id: roomId },
            dataType: 'json',
            success: function (response) {
                console.log('📦 Room Detail Response:', response);

                if (response.success) {
                    populateRoomDetail(response.data);
                    $('#roomDetailLoading').hide();
                    $('#roomDetailContent').show();
                } else {
                    showNotification('error', 'Lỗi', response.message || 'Không thể tải thông tin');
                    $('#roomDetailModal').modal('hide');
                }
            },
            error: function (xhr, error) {
                console.error('❌ Load room detail error:', error);
                showNotification('error', 'Lỗi', 'Không thể tải thông tin phòng');
                $('#roomDetailModal').modal('hide');
            }
        });
    }

    function populateRoomDetail(room) {
        // Room info
        $('#detailTieuDe').text(room.tieuDe || 'Chưa có tiêu đề');
        $('#detailDienTich').text(room.dienTich ? room.dienTich + ' m²' : '-');
        $('#detailGiaTien').text(formatCurrency(room.giaTien || 0));
        $('#detailTienCoc').text(formatCurrency(room.tienCoc || 0));
        $('#detailSoNguoi').text(room.soNguoiToiDa || '-');
        $('#detailChuTro').text(room.chuTroName || 'Chưa có thông tin');
        $('#detailNhaTro').text(room.nhaTroName || 'Chưa có thông tin');
        $('#detailTrangThai').text(room.trangThai || '-');

        // Load images if available
        var imageGrid = $('#roomImageGrid');
        imageGrid.empty();
        if (room.images && room.images.length > 0) {
            room.images.forEach(function (img) {
                imageGrid.append(
                    '<div class="room-image-card">' +
                    '<img src="' + img.url + '" alt="Room image" />' +
                    '</div>'
                );
            });
        } else {
            imageGrid.append('<p class="text-muted">Chưa có hình ảnh</p>');
        }

        // Clear reject reason
        $('#rejectReason').val('');

        // Update modal buttons based on status
        var isPending = !room.isDuyet && !room.isBiKhoa;
        $('#btnModalApprove, #btnModalReject').prop('disabled', !isPending);
    }

    // ============ APPROVE/REJECT/LOCK ============
    function approveRoom() {
        if (!currentRoomId) {
            showNotification('error', 'Lỗi', 'Không thể xác định phòng');
            return;
        }

        swal({
            title: 'Duyệt phòng?',
            text: 'Sau khi duyệt, phòng sẽ xuất hiện trên hệ thống.',
            icon: 'info',
            buttons: {
                cancel: {
                    text: 'Hủy',
                    visible: true,
                    className: 'btn btn-secondary'
                },
                confirm: {
                    text: 'Duyệt',
                    className: 'btn btn-success'
                }
            }
        }).then(function (result) {
            if (result) {
                performApprove(currentRoomId);
            }
        });
    }

    function quickApprove(roomId) {
        swal({
            title: 'Duyệt nhanh?',
            text: 'Bạn có chắc muốn duyệt phòng này?',
            icon: 'info',
            buttons: {
                cancel: {
                    text: 'Hủy',
                    visible: true,
                    className: 'btn btn-secondary'
                },
                confirm: {
                    text: 'Duyệt',
                    className: 'btn btn-success'
                }
            }
        }).then(function (result) {
            if (result) {
                performApprove(roomId);
            }
        });
    }

    function performApprove(roomId) {
        swal({
            title: 'Đang xử lý...',
            text: 'Vui lòng chờ',
            icon: 'info',
            buttons: false,
            closeOnClickOutside: false,
            closeOnEsc: false
        });

        $.ajax({
            url: apiUrls.approveRoom,
            type: 'POST',
            data: { id: roomId },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    swal({
                        title: 'Thành công!',
                        text: response.message || 'Đã duyệt phòng',
                        icon: 'success',
                        button: 'OK'
                    }).then(function () {
                        $('#roomDetailModal').modal('hide');
                        roomsTable.ajax.reload(null, false);
                        loadStats();
                    });
                } else {
                    swal('Lỗi', response.message || 'Không thể duyệt phòng', 'error');
                }
            },
            error: function (xhr, error) {
                console.error('❌ Approve error:', error);
                swal('Lỗi', 'Có lỗi xảy ra khi duyệt phòng', 'error');
            }
        });
    }

    function rejectRoom() {
        if (!currentRoomId) {
            showNotification('error', 'Lỗi', 'Không thể xác định phòng');
            return;
        }

        var reason = $('#rejectReason').val().trim();
        if (!reason) {
            showNotification('warning', 'Cảnh báo', 'Vui lòng nhập lý do từ chối');
            $('#rejectReason').focus().addClass('is-invalid');
            setTimeout(function () {
                $('#rejectReason').removeClass('is-invalid');
            }, 3000);
            return;
        }

        swal({
            title: 'Từ chối phòng?',
            text: 'Lý do: ' + reason,
            icon: 'warning',
            buttons: {
                cancel: {
                    text: 'Hủy',
                    visible: true,
                    className: 'btn btn-secondary'
                },
                confirm: {
                    text: 'Từ chối',
                    className: 'btn btn-danger'
                }
            },
            dangerMode: true
        }).then(function (result) {
            if (result) {
                performReject(currentRoomId, reason);
            }
        });
    }

    function quickReject(roomId) {
        swal({
            title: 'Lý do từ chối',
            text: 'Vui lòng nhập lý do từ chối:',
            content: {
                element: 'input',
                attributes: {
                    placeholder: 'Ví dụ: Hình ảnh không phù hợp...',
                    type: 'text'
                }
            },
            buttons: {
                cancel: {
                    text: 'Hủy',
                    visible: true,
                    className: 'btn btn-secondary'
                },
                confirm: {
                    text: 'Từ chối',
                    className: 'btn btn-danger'
                }
            },
            dangerMode: true
        }).then(function (reason) {
            if (reason === null) return;

            if (reason === '' || !reason.trim()) {
                showNotification('warning', 'Cảnh báo', 'Vui lòng nhập lý do từ chối');
                return;
            }
            performReject(roomId, reason.trim());
        });
    }

    function performReject(roomId, reason) {
        swal({
            title: 'Đang xử lý...',
            text: 'Vui lòng chờ',
            icon: 'info',
            buttons: false,
            closeOnClickOutside: false,
            closeOnEsc: false
        });

        $.ajax({
            url: apiUrls.rejectRoom,
            type: 'POST',
            data: { id: roomId, reason: reason },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    swal({
                        title: 'Thành công!',
                        text: response.message || 'Đã từ chối phòng',
                        icon: 'success',
                        button: 'OK'
                    }).then(function () {
                        $('#roomDetailModal').modal('hide');
                        roomsTable.ajax.reload(null, false);
                        loadStats();
                    });
                } else {
                    swal('Lỗi', response.message || 'Không thể từ chối', 'error');
                }
            },
            error: function (xhr, error) {
                console.error('❌ Reject error:', error);
                swal('Lỗi', 'Có lỗi xảy ra khi từ chối', 'error');
            }
        });
    }

    function toggleLock(roomId, shouldLock) {
        var action = shouldLock ? 'khóa' : 'mở khóa';
        
        swal({
            title: shouldLock ? 'Khóa phòng?' : 'Mở khóa phòng?',
            text: 'Bạn có chắc muốn ' + action + ' phòng này?',
            icon: 'warning',
            buttons: {
                cancel: {
                    text: 'Hủy',
                    visible: true,
                    className: 'btn btn-secondary'
                },
                confirm: {
                    text: shouldLock ? 'Khóa' : 'Mở khóa',
                    className: shouldLock ? 'btn btn-warning' : 'btn btn-success'
                }
            }
        }).then(function (result) {
            if (result) {
                performToggleLock(roomId, shouldLock);
            }
        });
    }

    function performToggleLock(roomId, shouldLock) {
        swal({
            title: 'Đang xử lý...',
            text: 'Vui lòng chờ',
            icon: 'info',
            buttons: false,
            closeOnClickOutside: false,
            closeOnEsc: false
        });

        $.ajax({
            url: apiUrls.toggleLockRoom,
            type: 'POST',
            data: { id: roomId, isLocked: shouldLock },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    swal({
                        title: 'Thành công!',
                        text: response.message,
                        icon: 'success',
                        button: 'OK'
                    }).then(function () {
                        roomsTable.ajax.reload(null, false);
                        loadStats();
                    });
                } else {
                    swal('Lỗi', response.message || 'Không thể thực hiện', 'error');
                }
            },
            error: function (xhr, error) {
                console.error('❌ Toggle lock error:', error);
                swal('Lỗi', 'Có lỗi xảy ra', 'error');
            }
        });
    }

    // ============ STATS ============
    function loadStats() {
        if (!apiUrls.getRoomStats) return;

        $.ajax({
            url: apiUrls.getRoomStats,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (response.success && response.data) {
                    console.log('✅ Room stats loaded:', response.data);
                    // Update sidebar stats if needed
                    if (typeof updateRoomStats === 'function') {
                        updateRoomStats(response.data);
                    }
                }
            },
            error: function () {
                console.log('⚠️ Could not load stats');
            }
        });
    }

    // ============ HELPER FUNCTIONS ============
    function formatDate(date) {
        if (!date || isNaN(date.getTime())) return '-';

        var day = String(date.getDate()).padStart(2, '0');
        var month = String(date.getMonth() + 1).padStart(2, '0');
        var year = date.getFullYear();

        return day + '/' + month + '/' + year;
    }

    function getRelativeTime(date) {
        if (!date || isNaN(date.getTime())) return '';

        var now = new Date();
        var diff = now - date;
        var seconds = Math.floor(diff / 1000);
        var minutes = Math.floor(seconds / 60);
        var hours = Math.floor(minutes / 60);
        var days = Math.floor(hours / 24);

        if (days > 30) {
            return Math.floor(days / 30) + ' tháng trước';
        } else if (days > 0) {
            return days + ' ngày trước';
        } else if (hours > 0) {
            return hours + ' giờ trước';
        } else if (minutes > 0) {
            return minutes + ' phút trước';
        } else {
            return 'Vừa xong';
        }
    }

    function formatCurrency(amount) {
        if (!amount || amount <= 0) return '0 đ';

        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    }

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
        approveRoom: approveRoom,
        rejectRoom: rejectRoom,
        quickApprove: quickApprove,
        quickReject: quickReject,
        toggleLock: toggleLock,
        applyFilter: applyFilter,
        refreshTable: refreshTable
    };

})(jQuery);
