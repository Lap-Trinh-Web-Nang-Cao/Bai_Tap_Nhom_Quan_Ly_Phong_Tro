/**
 * Support page JS - server-side DataTable + actions
 * Mirrors UsersPage pattern for paging, filters and operations
 */
var SupportPage = (function ($) {
    'use strict';

    var table = null;
    var config = {
        urls: {
            getSupports: '',
            getSupportDetail: '',
            process: '',
            complete: '',
            reject: '',
            getStatistics: ''
        }
    };

    function init(options) {
        console.log('🚀 SupportPage.init() called with options:', options);
        config = $.extend(true, config, options || {});
        console.log('✅ Config updated:', config);
        
        initDataTable();
        initFilters();
        
        // Load stats AFTER table is ready (brief delay to ensure DOM is ready)
        setTimeout(function () {
            console.log('📊 Loading initial statistics...');
            loadStatistics();
        }, 500);
    }

    function initIndex(options) {
        // Alias for init - called from view
        init(options);
    }

    function initDataTable() {
        table = $('#supportTable').DataTable({
            processing: true,
            serverSide: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            order: [[4, 'desc']],
            dom: '<"top">rt<"bottom"ip><"clear">',
            ajax: {
                url: config.urls.getSupports,
                type: 'POST',
                data: function (d) {
                    d.search = $('#searchInput').val() || '';
                    d.status = $('#statusFilter').val() || '';
                    d.type = $('#typeFilter').val() || '';
                    console.log('📤 DataTable sending:', { draw: d.draw, start: d.start, length: d.length, search: d.search, status: d.status, type: d.type });
                },
                dataSrc: function (json) {
                    console.log('📥 DataTable received:', json);
                    if (!json) return [];
                    if (Array.isArray(json.data)) return json.data;
                    if (Array.isArray(json.items)) return json.items;
                    if (json.Items && Array.isArray(json.Items)) return json.Items;
                    return [];
                },
                error: function (xhr, status, error) {
                    console.error('❌ DataTable error:', { status: status, error: error });
                    showNotification('error', 'Lỗi', 'Không thể tải dữ liệu yêu cầu hỗ trợ');
                }
            },
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/vi.json'
            },
            columns: [
                { data: null },               // index
                { data: 'TenLoaiHoTro' },     // Loại
                { data: 'TieuDe' },           // Tiêu đề
                { data: 'TenPhong' },         // Phòng
                { data: 'ThoiGianTao' },      // Thời gian
                { data: 'TrangThai' },        // Trạng thái
                { data: 'HoTroId' }           // actions
            ],
            columnDefs: [
                { orderable: false, targets: [0, 6] },
                { className: 'text-center', targets: [0, 5, 6] },
                {
                    targets: 0,
                    render: function (data, type, row, meta) {
                        var start = meta.settings._iDisplayStart || 0;
                        return start + meta.row + 1;
                    }
                },
                {
                    targets: 2,
                    render: function (data, type, row) {
                        var title = data || 'Không có tiêu đề';
                        var short = (row.MoTa || '').substring(0, 80);
                        return '<strong>' + escapeHtml(title) + '</strong>' + (short ? '<br/><small class="text-muted">' + escapeHtml(short) + '</small>' : '');
                    }
                },
                {
                    targets: 4,
                    render: function (data) {
                        if (!data) return '<span class="text-muted">N/A</span>';
                        var d = new Date(data);
                        if (!isNaN(d.getTime())) {
                            return pad(d.getDate()) + '/' + pad(d.getMonth() + 1) + '/' + d.getFullYear() + '<br/><small class="text-muted">' + pad(d.getHours()) + ':' + pad(d.getMinutes()) + '</small>';
                        }
                        return escapeHtml(data);
                    }
                },
                {
                    targets: 5,
                    render: function (data) {
                        switch ((data || '').toString()) {
                            case 'Moi': return '<span class="badge badge-info">Mới gửi</span>';
                            case 'DangXuLy': return '<span class="badge badge-warning">Đang xử lý</span>';
                            case 'HoanThanh': return '<span class="badge badge-success">Hoàn thành</span>';
                            case 'TuChoi': return '<span class="badge badge-danger">Từ chối</span>';
                            default: return '<span class="badge badge-secondary">' + escapeHtml(data || 'N/A') + '</span>';
                        }
                    }
                },
                {
                    targets: 6,
                    render: function (data, type, row) {
                        var id = data;
                        var html = '<div class="btn-group btn-group-sm" role="group">';
                        html += '<button class="btn btn-info" onclick="SupportPage.viewDetail(\'' + id + '\')" title="Xem chi tiết"><i class="fa fa-eye"></i></button>';
                        if (row.TrangThai === 'Moi') {
                            html += '<button class="btn btn-warning" onclick="SupportPage.process(\'' + id + '\')" title="Bắt đầu xử lý"><i class="fa fa-play"></i></button>';
                        }
                        if (row.TrangThai === 'Moi' || row.TrangThai === 'DangXuLy') {
                            html += '<button class="btn btn-success" onclick="SupportPage.complete(\'' + id + '\')" title="Hoàn thành"><i class="fa fa-check"></i></button>';
                            html += '<button class="btn btn-danger" onclick="SupportPage.reject(\'' + id + '\')" title="Từ chối"><i class="fa fa-times"></i></button>';
                        }
                        html += '</div>';
                        return html;
                    }
                }
            ],
            drawCallback: function () {
                console.log('✅ DataTable redrawn');
                $('[data-toggle="tooltip"]').tooltip();
            }
        });
        console.log('✅ DataTable initialized');
    }

    function initFilters() {
        console.log('🔧 Initializing filter handlers...');
        
        // Search button click
        $('#btnSearchSupport').on('click', function () {
            console.log('🔘 Search button clicked');
            if (table) table.ajax.reload(function () {
                console.log('✅ Table reloaded after search');
                loadStatistics();
            }, false);
        });

        // Search input - Enter key
        $('#searchInput').on('keyup', function (e) {
            if (e.keyCode === 13) {
                console.log('⌨️ Enter pressed in search');
                if (table) table.ajax.reload(function () {
                    loadStatistics();
                }, false);
            }
        });

        // Status and Type filters - auto reload on change
        $('#statusFilter, #typeFilter').on('change', function () {
            console.log('🔘 Filter dropdown changed:', $(this).attr('id'));
            if (table) table.ajax.reload(function () {
                loadStatistics();
            }, false);
        });
        
        console.log('✅ Filter handlers initialized');
    }

    function viewDetail(id) {
        if (!id) return;
        $('#detailModal').modal('show');
        $('#detailContent').html('<div class="text-center py-5"><div class="spinner-border text-primary"></div><p class="mt-2">Đang tải...</p></div>');
        $.get(config.urls.getSupportDetail, { id: id })
            .done(function (res) {
                if (res.success) {
                    buildDetail(res.data);
                } else {
                    $('#detailContent').html('<div class="alert alert-danger">' + escapeHtml(res.message || 'Lỗi') + '</div>');
                }
            })
            .fail(function () {
                $('#detailContent').html('<div class="alert alert-danger">Lỗi khi tải chi tiết</div>');
            });
    }

    function buildDetail(data) {
        var html = '<div class="row"><div class="col-md-6"><label class="text-muted">Mã yêu cầu</label><p class="font-weight-bold">#' + (data.SoYeuCau || '') + '</p></div>';
        html += '<div class="col-md-6"><label class="text-muted">Trạng thái</label><p>' + getStatusBadge(data.TrangThai) + '</p></div></div>';
        html += '<hr><div class="form-group"><label class="text-muted">Tiêu đề</label><h5>' + escapeHtml(data.TieuDe || '') + '</h5></div>';
        html += '<div class="form-group"><label class="text-muted">Mô tả</label><div class="p-3 bg-light rounded">' + escapeHtml(data.MoTa || '-') + '</div></div>';
        if (data.TenPhong || data.PhongId) {
            html += '<div class="form-group"><label class="text-muted">Phòng liên quan</label><p>' + escapeHtml(data.TenPhong || ('Phòng #' + (data.PhongId || '').toString().substr(0, 8))) + '</p></div>';
        }
        $('#detailContent').html(html);

        var footer = '<button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>';
        if (data.TrangThai === 'Moi') {
            footer += ' <button type="button" class="btn btn-warning" onclick="$(\'#detailModal\').modal(\'hide\'); SupportPage.process(\'' + data.HoTroId + '\')"><i class="fa fa-play"></i> Bắt đầu xử lý</button>';
        }
        if (data.TrangThai === 'Moi' || data.TrangThai === 'DangXuLy') {
            footer += ' <button type="button" class="btn btn-success" onclick="$(\'#detailModal\').modal(\'hide\'); SupportPage.complete(\'' + data.HoTroId + '\')"><i class="fa fa-check"></i> Hoàn thành</button>';
            footer += ' <button type="button" class="btn btn-danger" onclick="$(\'#detailModal\').modal(\'hide\'); SupportPage.reject(\'' + data.HoTroId + '\')"><i class="fa fa-times"></i> Từ chối</button>';
        }
        $('#detailFooter').html(footer);
    }

    function process(id) {
        if (!id) return;
        swal({
            title: "Bắt đầu xử lý?",
            icon: "info",
            buttons: true
        }).then(function (ok) {
            if (!ok) return;
            $.post(config.urls.process, { id: id })
                .done(function (res) {
                    if (res.success) swal("Thành công", res.message, "success").then(function () { table.ajax.reload(); loadStatistics(); });
                    else swal("Lỗi", res.message || 'Không thể cập nhật', "error");
                })
                .fail(function () { swal("Lỗi", "Không thể cập nhật", "error"); });
        });
    }

    function complete(id) {
        if (!id) return;
        swal({
            title: "Xác nhận hoàn thành?",
            icon: "success",
            buttons: true
        }).then(function (ok) {
            if (!ok) return;
            $.post(config.urls.complete, { id: id })
                .done(function (res) {
                    if (res.success) swal("Thành công", res.message, "success").then(function () { table.ajax.reload(); loadStatistics(); });
                    else swal("Lỗi", res.message || 'Không thể cập nhật', "error");
                })
                .fail(function () { swal("Lỗi", "Không thể cập nhật", "error"); });
        });
    }

    function reject(id) {
        if (!id) return;
        swal({
            text: 'Nhập lý do từ chối:',
            content: "input",
            button: { text: "Xác nhận", closeModal: false }
        }).then(function (reason) {
            if (!reason) { swal("Lỗi!", "Vui lòng nhập lý do", "error"); return; }
            $.post(config.urls.reject, { id: id, lyDo: reason })
                .done(function (res) {
                    if (res.success) swal("Thành công", res.message, "success").then(function () { table.ajax.reload(); loadStatistics(); });
                    else swal("Lỗi", res.message || 'Không thể từ chối', "error");
                })
                .fail(function () { swal("Lỗi", "Không thể từ chối", "error"); });
        });
    }

    function clearFilters() {
        $('#statusFilter').val('');
        $('#typeFilter').val('');
        $('#searchInput').val('');
        if (table) table.ajax.reload();
    }

    function refreshTable() {
        if (table) table.ajax.reload();
        loadStatistics();
    }

    function loadStatistics() {
        if (!config.urls.getStatistics) {
            console.warn('⚠️ getStatistics URL not configured');
            return;
        }

        var filters = {
            search: $('#searchInput').val() || '',
            status: $('#statusFilter').val() || '',
            type: $('#typeFilter').val() || ''
        };
        
        console.log('📊 Loading statistics with filters:', filters);

        $.ajax({
            url: config.urls.getStatistics,
            type: 'GET',
            data: filters,
            dataType: 'json',
            timeout: 10000,
            success: function (res) {
                console.log('✅ Statistics loaded:', res);
                if (res && res.success && res.data) {
                    updateStatsUI(res.data);
                } else {
                    console.warn('⚠️ Statistics response invalid:', res);
                }
            },
            error: function (xhr, status, error) {
                console.error('❌ Statistics error:', { status: status, error: error, response: xhr.responseText });
            }
        });
    }

    function updateStatsUI(stats) {
        console.log('🎨 Updating stats UI with:', stats);
        
        // Update stats cards if they exist in the page
        var totalElement = document.querySelector('[data-stat="total"]');
        var newElement = document.querySelector('[data-stat="new"]');
        var processingElement = document.querySelector('[data-stat="processing"]');
        var completedElement = document.querySelector('[data-stat="completed"]');

        if (totalElement) {
            console.log('📊 Updating total:', stats.TotalRequests);
            totalElement.textContent = stats.TotalRequests || 0;
        }
        if (newElement) {
            console.log('📬 Updating new:', stats.NewRequests);
            newElement.textContent = stats.NewRequests || 0;
        }
        if (processingElement) {
            console.log('⚙️ Updating processing:', stats.ProcessingRequests);
            processingElement.textContent = stats.ProcessingRequests || 0;
        }
        if (completedElement) {
            console.log('✅ Updating completed:', stats.CompletedRequests);
            completedElement.textContent = stats.CompletedRequests || 0;
        }
    }

    // helpers
    function showNotification(type, title, message) {
        if (typeof swal !== 'undefined') {
            swal(title, message, type);
        } else {
            alert(title + ': ' + message);
        }
    }

    function getStatusBadge(status) {
        switch ((status || '').toString()) {
            case 'Moi': return '<span class="badge badge-info">Mới gửi</span>';
            case 'DangXuLy': return '<span class="badge badge-warning">Đang xử lý</span>';
            case 'HoanThanh': return '<span class="badge badge-success">Hoàn thành</span>';
            case 'TuChoi': return '<span class="badge badge-danger">Từ chối</span>';
            default: return '<span class="badge badge-secondary">' + escapeHtml(status || 'N/A') + '</span>';
        }
    }
    function pad(n) { return n < 10 ? '0' + n : n; }
    function escapeHtml(str) {
        if (!str && str !== 0) return '';
        return String(str).replace(/[&<>"'\/]/g, function (s) {
            var entityMap = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;', '/': '&#x2F;' };
            return entityMap[s];
        });
    }

    return {
        init: init,
        initIndex: initIndex,
        viewDetail: viewDetail,
        process: process,
        complete: complete,
        reject: reject,
        clearFilters: clearFilters,
        refreshTable: refreshTable,
        loadStatistics: loadStatistics
    };
})(jQuery);

// Backward compatibility for inline onclicks
function clearFilters() { if (window.SupportPage) window.SupportPage.clearFilters(); }
function refreshTable() { if (window.SupportPage) window.SupportPage.refreshTable(); }