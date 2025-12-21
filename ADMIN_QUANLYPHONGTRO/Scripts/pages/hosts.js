/**
 * ========================================
 * HOSTS PAGE JAVASCRIPT
 * Xét duyệt chủ trọ - Admin Panel
 * ========================================
 */

// Namespace để tránh xung đột
var HostsPage = (function ($) {
    'use strict';

    // ============ PRIVATE VARIABLES ============
    var hostsTable = null;
    var currentHostId = null;
    var currentFilter = {
        status: 'pending', // Mặc định filter pending
        keyword: ''
    };

    // API URLs - sẽ được set từ view
    var apiUrls = {
        getPendingHosts: '',
        getHostDetail: '',
        approveHost: '',
        rejectHost: '',
        getHostStats: ''
    };

    // ============ INITIALIZATION ============
    function init(urls) {
        apiUrls = $.extend(apiUrls, urls);

        initDataTable();
        initFilterEvents();
        initModalEvents();
        initLightbox();
        loadStats();

        console.log('✅ HostsPage initialized');
    }

    // ============ DATATABLE ============
    function initDataTable() {
        hostsTable = $('#hostsTable').DataTable({
            processing: true,
            serverSide: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            order: [[4, 'desc']], // Sắp xếp theo ngày đăng ký
            dom: '<"top"lf>rt<"bottom"ip><"clear">',
            ajax: {
                url: apiUrls.getPendingHosts,
                type: 'POST',
                dataType: 'json',
                data: function (d) {
                    // Thêm filter parameters
                    d.status = currentFilter.status;
                    d.keyword = currentFilter.keyword;
                    console.log('📤 Sending filter:', d);
                },
                dataSrc: function (json) {
                    // Log để debug
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
                { data: null, name: 'host' },           // 0: Chủ trọ
                { data: 'DienThoai', name: 'phone' },   // 1: Số điện thoại
                { data: 'SoCCCD', name: 'idCard' },     // 2: Số CCCD
                { data: null, name: 'documents' },      // 3: Giấy tờ
                { data: 'NgayDangKy', name: 'date' },   // 4: Ngày đăng ký
                { data: 'TrangThaiXacThuc', name: 'status' }, // 5: Trạng thái
                { data: null, name: 'actions' }         // 6: Thao tác
            ],
            columnDefs: [
                { orderable: false, targets: [0, 3, 6] },
                { className: 'text-center', targets: [6] },
                {
                    targets: 0,
                    render: function (data, type, row) {
                        return renderHostInfo(row);
                    }
                },
                {
                    targets: 1,
                    render: function (data, type, row) {
                        return renderPhoneNumber(row.DienThoai);
                    }
                },
                {
                    targets: 2,
                    render: function (data, type, row) {
                        return renderIdCard(row.SoCCCD);
                    }
                },
                {
                    targets: 3,
                    render: function (data, type, row) {
                        return renderDocuments(row);
                    }
                },
                {
                    targets: 4,
                    render: function (data, type, row) {
                        return renderDate(row.NgayDangKy);
                    }
                },
                {
                    targets: 5,
                    render: function (data, type, row) {
                        return renderStatus(row.TrangThaiXacThuc);
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
    function renderHostInfo(row) {
        var avatar = row.Avatar || '/Content/img/default-avatar.png';
        var name = escapeHtml(row.HoTen || 'Chưa cập nhật');
        var email = escapeHtml(row.Email || '');

        return '<div class="host-info">' +
            '<img src="' + avatar + '" alt="' + name + '" class="host-avatar" ' +
            'onerror="this.onerror=null; this.src=\'/Content/img/default-avatar.png\';" />' +
            '<div class="host-details">' +
            '<div class="host-name" title="' + name + '">' + name + '</div>' +
            '<div class="host-email" title="' + email + '">' + (email || '<span class="text-muted">Chưa có email</span>') + '</div>' +
            '</div>' +
            '</div>';
    }

    function renderPhoneNumber(phone) {
        if (!phone) {
            return '<span class="phone-number empty">Chưa cập nhật</span>';
        }
        return '<span class="phone-number">' + formatPhoneNumber(phone) + '</span>';
    }

    function renderIdCard(idCard) {
        if (!idCard) {
            return '<span class="id-card-number empty"><i class="fa fa-exclamation-circle"></i> Chưa cập nhật</span>';
        }
        return '<span class="id-card-number">' + formatIdCard(idCard) + '</span>';
    }

    function renderDocuments(row) {
        var hasDocuments = row.DaTaiGiayTo;
        var count = row.SoTapTinDinhKem || 0;

        if (hasDocuments && count > 0) {
            return '<div class="documents-status">' +
                '<div class="doc-icon uploaded"><i class="fa fa-check"></i></div>' +
                '<span class="doc-text uploaded">Đã tải ' + count + ' ảnh</span>' +
                '</div>';
        }

        return '<div class="documents-status">' +
            '<div class="doc-icon missing"><i class="fa fa-times"></i></div>' +
            '<span class="doc-text missing">Chưa tải</span>' +
            '</div>';
    }

    function renderDate(dateString) {
        if (!dateString) return '<span class="text-muted">-</span>';

        var date = new Date(dateString);
        if (isNaN(date.getTime())) return '<span class="text-muted">-</span>';

        var formattedDate = formatDate(date);
        var formattedTime = formatTime(date);
        var relativeTime = getRelativeTime(date);

        return '<div class="register-date">' +
            formattedDate +
            '<span class="date-time" title="' + formattedTime + '">' + relativeTime + '</span>' +
            '</div>';
    }

    function renderStatus(status) {
        var statusClass = 'pending';
        var statusIcon = 'fa-clock';
        var statusText = 'Chờ duyệt';

        switch (status) {
            case 'Đã xác minh':
            case 'approved':
                statusClass = 'approved';
                statusIcon = 'fa-check-circle';
                statusText = 'Đã xác minh';
                break;
            case 'Từ chối':
            case 'Đã từ chối':
            case 'rejected':
                statusClass = 'rejected';
                statusIcon = 'fa-times-circle';
                statusText = 'Từ chối';
                break;
            default:
                statusClass = 'pending';
                statusIcon = 'fa-clock';
                statusText = 'Chờ duyệt';
        }

        return '<span class="status-badge ' + statusClass + '">' +
            '<i class="fa ' + statusIcon + '"></i> ' + statusText +
            '</span>';
    }

    function renderActions(row) {
        var hostId = row.NguoiDungId;
        var status = row.TrangThaiXacThuc;
        var isPending = status === 'Chờ duyệt' || status === 'pending' || !status;

        var html = '<div class="action-buttons">';

        // Nút xem chi tiết - luôn hiển thị
        html += '<button type="button" class="btn-action btn-view" ' +
            'onclick="HostsPage.viewDetail(\'' + hostId + '\')" ' +
            'data-toggle="tooltip" data-placement="top" title="Xem chi tiết">' +
            '<i class="fa fa-eye"></i>' +
            '</button>';

        // Nếu đang chờ duyệt, hiển thị nút duyệt/từ chối
        if (isPending) {
            html += '<button type="button" class="btn-action btn-approve" ' +
                'onclick="HostsPage.quickApprove(\'' + hostId + '\')" ' +
                'data-toggle="tooltip" data-placement="top" title="Xác thực nhanh">' +
                '<i class="fa fa-check"></i>' +
                '</button>';

            html += '<button type="button" class="btn-action btn-reject" ' +
                'onclick="HostsPage.quickReject(\'' + hostId + '\')" ' +
                'data-toggle="tooltip" data-placement="top" title="Từ chối">' +
                '<i class="fa fa-times"></i>' +
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
        currentFilter.status = $('#filterStatus').val();
        currentFilter.keyword = $('#filterKeyword').val().trim();

        console.log('🔍 Applying filter:', currentFilter);
        if (hostsTable) {
            hostsTable.ajax.reload(null, false);
        }
    }

    // ============ MODAL EVENTS ============
    function initModalEvents() {
        // Modal close - reset currentHostId
        $('#hostDetailModal').on('hidden.bs.modal', function () {
            currentHostId = null;
            $('#rejectReason').val('');
            $('#modalStatusNote').hide();
        });
    }

    // ============ VIEW DETAIL ============
    function viewDetail(hostId) {
        currentHostId = hostId;

        // Show modal with loading state
        $('#hostDetailModal').modal('show');
        $('#hostDetailLoading').show();
        $('#hostDetailContent').hide();

        // Load host detail
        $.ajax({
            url: apiUrls.getHostDetail,
            type: 'GET',
            data: { id: hostId },
            dataType: 'json',
            success: function (response) {
                console.log('📦 Host Detail Response:', response);

                if (response.success) {
                    populateHostDetail(response.data);
                    $('#hostDetailLoading').hide();
                    $('#hostDetailContent').show();
                } else {
                    showNotification('error', 'Lỗi', response.message || 'Không thể tải thông tin');
                    $('#hostDetailModal').modal('hide');
                }
            },
            error: function (xhr, error) {
                console.error('❌ Load host detail error:', error);
                showNotification('error', 'Lỗi', 'Không thể tải thông tin chủ trọ');
                $('#hostDetailModal').modal('hide');
            }
        });
    }

    function populateHostDetail(host) {
        // Personal info
        $('#detailHoTen').val(host.HoTen || 'Chưa cập nhật');
        $('#detailEmail').val(host.Email || 'Chưa có email');
        $('#detailDienThoai').val(formatPhoneNumber(host.DienThoai) || 'Chưa cập nhật');
        $('#detailSoCCCD').val(host.SoCCCD || 'Chưa cập nhật');
        $('#detailNgaySinh').val(host.NgaySinh ? formatDate(new Date(host.NgaySinh)) : 'Chưa cập nhật');
        $('#detailQueQuan').val(host.QueQuan || 'Chưa cập nhật');

        // Images
        setImageWithFallback('#detailCCCDFront', host.CCCDMatTruoc);
        setImageWithFallback('#detailCCCDBack', host.CCCDMatSau);
        setImageWithFallback('#detailBusinessLicense', host.GiayPhepKinhDoanh);

        // Clear reject reason
        $('#rejectReason').val('');

        // Update modal buttons based on status
        var isPending = host.TrangThaiXacThuc === 'Chờ duyệt' || !host.TrangThaiXacThuc;
        $('#btnModalApprove, #btnModalReject').prop('disabled', !isPending);

        if (!isPending) {
            var statusText = host.TrangThaiXacThuc === 'Đã xác minh' ?
                '<i class="fa fa-check-circle text-success mr-1"></i> Đã xác minh' :
                '<i class="fa fa-times-circle text-danger mr-1"></i> Đã từ chối';
            $('#modalStatusNote').html(statusText).show();
        } else {
            $('#modalStatusNote').hide();
        }
    }

    function setImageWithFallback(selector, imageUrl) {
        var $img = $(selector);
        var fallbackUrl = '/Content/img/no-image.png';

        if (imageUrl && imageUrl !== fallbackUrl && imageUrl.length > 0) {
            $img.attr('src', imageUrl)
                .off('error')
                .on('error', function () {
                    $(this).attr('src', fallbackUrl);
                });
        } else {
            $img.attr('src', fallbackUrl);
        }
    }

    // ============ APPROVE/REJECT ============
    function approveHost() {
        if (!currentHostId) {
            showNotification('error', 'Lỗi', 'Không thể xác định chủ trọ');
            return;
        }

        swal({
            title: 'Xác thực chủ trọ?',
            text: 'Sau khi xác thực, chủ trọ sẽ được phép đăng phòng trọ lên hệ thống.',
            icon: 'info',
            buttons: {
                cancel: {
                    text: 'Hủy',
                    visible: true,
                    className: 'btn btn-secondary'
                },
                confirm: {
                    text: 'Xác thực',
                    className: 'btn btn-success'
                }
            }
        }).then(function (result) {
            if (result) {
                performApprove(currentHostId);
            }
        });
    }

    function quickApprove(hostId) {
        swal({
            title: 'Xác thực nhanh?',
            text: 'Bạn có chắc muốn xác thực chủ trọ này?',
            icon: 'info',
            buttons: {
                cancel: {
                    text: 'Hủy',
                    visible: true,
                    className: 'btn btn-secondary'
                },
                confirm: {
                    text: 'Xác thực',
                    className: 'btn btn-success'
                }
            }
        }).then(function (result) {
            if (result) {
                performApprove(hostId);
            }
        });
    }

    function performApprove(hostId) {
        // Show loading
        swal({
            title: 'Đang xử lý...',
            text: 'Vui lòng chờ',
            icon: 'info',
            buttons: false,
            closeOnClickOutside: false,
            closeOnEsc: false
        });

        $.ajax({
            url: apiUrls.approveHost,
            type: 'POST',
            data: { id: hostId },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    swal({
                        title: 'Thành công!',
                        text: response.message || 'Đã xác thực chủ trọ',
                        icon: 'success',
                        button: 'OK'
                    }).then(function () {
                        $('#hostDetailModal').modal('hide');
                        hostsTable.ajax.reload(null, false);
                        loadStats();
                    });
                } else {
                    swal('Lỗi', response.message || 'Không thể xác thực', 'error');
                }
            },
            error: function (xhr, error) {
                console.error('❌ Approve error:', error);
                swal('Lỗi', 'Có lỗi xảy ra khi xác thực', 'error');
            }
        });
    }

    function rejectHost() {
        if (!currentHostId) {
            showNotification('error', 'Lỗi', 'Không thể xác định chủ trọ');
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
            title: 'Từ chối chủ trọ?',
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
                performReject(currentHostId, reason);
            }
        });
    }

    function quickReject(hostId) {
        swal({
            title: 'Lý do từ chối',
            text: 'Vui lòng nhập lý do từ chối:',
            content: {
                element: 'input',
                attributes: {
                    placeholder: 'Ví dụ: Ảnh CCCD không rõ ràng...',
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
            if (reason === null) return; // Cancelled

            if (reason === '' || !reason.trim()) {
                showNotification('warning', 'Cảnh báo', 'Vui lòng nhập lý do từ chối');
                return;
            }
            performReject(hostId, reason.trim());
        });
    }

    function performReject(hostId, reason) {
        // Show loading
        swal({
            title: 'Đang xử lý...',
            text: 'Vui lòng chờ',
            icon: 'info',
            buttons: false,
            closeOnClickOutside: false,
            closeOnEsc: false
        });

        $.ajax({
            url: apiUrls.rejectHost,
            type: 'POST',
            data: { id: hostId, reason: reason },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    swal({
                        title: 'Thành công!',
                        text: response.message || 'Đã từ chối chủ trọ',
                        icon: 'success',
                        button: 'OK'
                    }).then(function () {
                        $('#hostDetailModal').modal('hide');
                        hostsTable.ajax.reload(null, false);
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

    // ============ STATS ============
    function loadStats() {
        if (!apiUrls.getHostStats) return;

        $.ajax({
            url: apiUrls.getHostStats,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (response.success && response.data) {
                    // Stats cards are removed, only update for any remaining uses
                    console.log('✅ Stats loaded:', response.data);
                } else if (response.pending !== undefined) {
                    console.log('✅ Stats loaded:', response);
                }
            },
            error: function () {
                console.log('⚠️ Could not load stats');
            }
        });
    }

    // ============ LIGHTBOX ============
    function initLightbox() {
        // Create lightbox element if not exists
        if ($('.lightbox-overlay').length === 0) {
            var $overlay = $('<div class="lightbox-overlay">' +
                '<div class="lightbox-content">' +
                '<span class="lightbox-close">&times;</span>' +
                '<img src="" alt="Preview" />' +
                '</div>' +
                '</div>');
            $('body').append($overlay);
        }

        // Open lightbox on image click
        $(document).on('click', '.doc-image', function () {
            var src = $(this).attr('src');
            if (src && src !== '/Content/img/no-image.png') {
                openLightbox(src);
            }
        });

        // Close lightbox on overlay click
        $(document).on('click', '.lightbox-overlay', function (e) {
            if (e.target === this) {
                closeLightbox();
            }
        });

        // Close lightbox on X click
        $(document).on('click', '.lightbox-close', function () {
            closeLightbox();
        });

        // Close on ESC key
        $(document).on('keyup', function (e) {
            if (e.keyCode === 27) {
                closeLightbox();
            }
        });
    }

    function openLightbox(imageSrc) {
        var $overlay = $('.lightbox-overlay');
        $overlay.find('img').attr('src', imageSrc);
        $overlay.addClass('active');
        $('body').css('overflow', 'hidden');
    }

    function closeLightbox() {
        $('.lightbox-overlay').removeClass('active');
        $('body').css('overflow', '');
    }

    // ============ HELPER FUNCTIONS ============
    function formatDate(date) {
        if (!date || isNaN(date.getTime())) return '-';

        var day = String(date.getDate()).padStart(2, '0');
        var month = String(date.getMonth() + 1).padStart(2, '0');
        var year = date.getFullYear();

        return day + '/' + month + '/' + year;
    }

    function formatTime(date) {
        if (!date || isNaN(date.getTime())) return '';

        var hours = String(date.getHours()).padStart(2, '0');
        var minutes = String(date.getMinutes()).padStart(2, '0');

        return hours + ':' + minutes;
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
            return formatTime(date);
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

    function formatPhoneNumber(phone) {
        if (!phone) return '';

        // Format: 0xxx xxx xxx
        var cleaned = phone.replace(/\D/g, '');
        if (cleaned.length === 10) {
            return cleaned.replace(/(\d{4})(\d{3})(\d{3})/, '$1 $2 $3');
        }
        return phone;
    }

    function formatIdCard(idCard) {
        if (!idCard) return '';

        // Format: xxx xxx xxx xxx (12 số)
        var cleaned = idCard.replace(/\D/g, '');
        if (cleaned.length === 12) {
            return cleaned.replace(/(\d{3})(\d{3})(\d{3})(\d{3})/, '$1 $2 $3 $4');
        }
        return idCard;
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
        approveHost: approveHost,
        rejectHost: rejectHost,
        quickApprove: quickApprove,
        quickReject: quickReject,
        applyFilter: applyFilter
    };

})(jQuery);
