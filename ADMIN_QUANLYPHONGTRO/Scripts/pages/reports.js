/**
 * Reports page JS - server-side DataTable + actions
 * Mirrors UsersPage pattern (server-side paging, filters, robust response parsing)
 */
var ReportsPage = (function ($) {
    'use strict';

    var table = null;
    var config = {
        urls: {
            getReports: '',
            getReportDetail: '',
            resolve: '',
            reject: '',
            delete: ''
        }
    };

    function init(options) {
        config = $.extend(true, config, options || {});
        initDataTable();
        initFilters();
        initModals();
    }

    function initDataTable() {
        table = $('#reportsTable').DataTable({
            processing: true,
            serverSide: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            order: [[3, 'desc']],
            dom: '<"top"lf>rt<"bottom"ip><"clear">',
            ajax: {
                url: config.urls.getReports,
                type: 'POST',
                data: function (d) {
                    // Add custom filters
                    d.status = $('#statusFilter').val() || '';
                    d.loaiThucThe = $('#typeFilter').val() || '';
                    d.search = $('#searchInput').val() || '';
                },
                dataSrc: function (json) {
                    // Support multiple shapes returned by backend
                    if (!json) return [];
                    if (Array.isArray(json.data)) return json.data;
                    if (Array.isArray(json.items)) return json.items;
                    // If backend returns wrapper with Items
                    if (json.Items && Array.isArray(json.Items)) return json.Items;
                    return [];
                },
                error: function (xhr, err, thrown) {
                    console.error('Reports DataTable error', err, thrown);
                    showNotification('error', 'Lỗi', 'Không thể tải dữ liệu báo cáo');
                }
            },
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/vi.json',
                processing: '<div class="spinner-border text-primary" role="status"><span class="sr-only">Đang tải...</span></div>',
                emptyTable: 'Không có báo cáo'
            },
            columns: [
                { data: null },                 // index
                { data: 'LoaiThucThe' },        // Loại
                { data: 'TieuDe' },             // Tiêu đề
                { data: 'ThoiGianBaoCao' },     // Thời gian
                { data: 'TrangThai' },          // Trạng thái
                { data: 'KetQua' },             // Kết quả
                { data: 'BaoCaoId' }            // actions
            ],
            columnDefs: [
                { orderable: false, targets: [0, 6] },
                { className: 'text-center', targets: [0, 4, 6] },
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
                        var desc = row.MoTa || '';
                        var short = desc.length > 80 ? desc.substring(0, 80) + '...' : desc;
                        return '<strong>' + escapeHtml(title) + '</strong>' +
                            (short ? '<br/><small class="text-muted">' + escapeHtml(short) + '</small>' : '');
                    }
                },
                {
                    targets: 3,
                    render: function (data) {
                        if (!data) return '<span class="text-muted">N/A</span>';
                        // Expect backend ISO format or formatted string
                        var d = new Date(data);
                        if (!isNaN(d.getTime())) {
                            return pad(d.getDate()) + '/' + pad(d.getMonth() + 1) + '/' + d.getFullYear() + ' <br/><small class="text-muted">' + pad(d.getHours()) + ':' + pad(d.getMinutes()) + '</small>';
                        }
                        return escapeHtml(data);
                    }
                },
                {
                    targets: 4,
                    render: function (data) {
                        switch ((data || '').toString()) {
                            case 'CHO_XU_LY': return '<span class="badge badge-warning">Chờ xử lý</span>';
                            case 'DANG_XU_LY': return '<span class="badge badge-info">Đang xử lý</span>';
                            case 'DA_XU_LY': return '<span class="badge badge-success">Đã xử lý</span>';
                            case 'TU_CHOI': return '<span class="badge badge-danger">Từ chối</span>';
                            default: return '<span class="badge badge-secondary">' + escapeHtml(data || 'N/A') + '</span>';
                        }
                    }
                },
                {
                    targets: 5,
                    render: function (data) {
                        if (!data) return '<span class="text-muted">-</span>';
                        return '<span title="' + escapeHtml(data) + '">' + escapeHtml(data.length > 40 ? data.substring(0, 40) + '...' : data) + '</span>';
                    }
                },
                {
                    targets: 6,
                    render: function (data, type, row) {
                        var id = data;
                        var html = '<div class="btn-group" role="group">';
                        html += '<button class="btn btn-sm btn-info" onclick="ReportsPage.viewDetail(\'' + id + '\')" title="Xem chi tiết"><i class="fa fa-eye"></i></button>';
                        if (row.TrangThai === 'CHO_XU_LY' || row.TrangThai === 'DANG_XU_LY') {
                            html += '<button class="btn btn-sm btn-success" onclick="ReportsPage.resolve(\'' + id + '\')" title="Xử lý"><i class="fa fa-check"></i></button>';
                            html += '<button class="btn btn-sm btn-warning" onclick="ReportsPage.reject(\'' + id + '\')" title="Từ chối"><i class="fa fa-ban"></i></button>';
                        }
                        html += '<button class="btn btn-sm btn-danger" onclick="ReportsPage.delete(\'' + id + '\')" title="Xóa"><i class="fa fa-trash"></i></button>';
                        html += '</div>';
                        return html;
                    }
                }
            ],
            drawCallback: function () {
                $('[data-toggle="tooltip"]').tooltip();
            }
        });
    }

    function initFilters() {
        $('#btnSearchReports').on('click', function () {
            if (table) table.ajax.reload();
        });
        $('#searchInput, #statusFilter, #typeFilter').on('change keyup', function (e) {
            if (e.type === 'keyup' && e.keyCode !== 13) return;
            if (table) table.ajax.reload();
        });
    }

    function initModals() {
        $('#reportDetailModal').on('hidden.bs.modal', function () {
            $('#reportDetailContent').html('');
        });
    }

    // Public action helpers
    function viewDetail(id) {
        if (!id) return;
        $('#reportDetailModal').modal('show');
        $('#reportDetailContent').html('<div class="text-center py-5"><div class="spinner-border text-primary"></div><p class="mt-2">Đang tải...</p></div>');
        $.get(config.urls.getReportDetail, { id: id })
            .done(function (res) {
                try {
                    if (res.success) buildDetail(res.data);
                    else $('#reportDetailContent').html('<div class="alert alert-danger">' + escapeHtml(res.message || 'Lỗi') + '</div>');
                } catch (err) {
                    $('#reportDetailContent').html('<div class="alert alert-danger">Lỗi khi xử lý dữ liệu</div>');
                }
            })
            .fail(function () {
                $('#reportDetailContent').html('<div class="alert alert-danger">Lỗi khi tải chi tiết</div>');
            });
    }

    function buildDetail(data) {
        var status = getStatusBadge(data.TrangThai);
        var type = getTypeBadge(data.LoaiThucThe);
        var html = '<div class="row"><div class="col-md-6"><label class="text-muted">Mã báo cáo</label><p class="font-weight-bold">#' + (data.SoBaoCao || '') + '</p></div>';
        html += '<div class="col-md-6"><label class="text-muted">Trạng thái</label><p>' + status + '</p></div></div>';
        html += '<hr><div class="form-group"><label class="text-muted">Tiêu đề</label><p class="font-weight-bold">' + escapeHtml(data.TieuDe || '') + '</p></div>';
        html += '<div class="form-group"><label class="text-muted">Mô tả</label><p>' + escapeHtml(data.MoTa || '-') + '</p></div>';
        if (data.KetQua) html += '<div class="form-group"><label class="text-muted">Kết quả</label><p>' + escapeHtml(data.KetQua) + '</p></div>';
        $('#reportDetailContent').html(html);
        // show buttons in modal footer
        if (data.TrangThai === 'CHO_XU_LY' || data.TrangThai === 'DANG_XU_LY') {
            $('#modalResolveBtn').show().off('click').on('click', function () {
                $('#reportDetailModal').modal('hide');
                resolve(data.BaoCaoId);
            });
            $('#modalRejectBtn').show().off('click').on('click', function () {
                $('#reportDetailModal').modal('hide');
                reject(data.BaoCaoId);
            });
        } else {
            $('#modalResolveBtn, #modalRejectBtn').hide();
        }
    }

    function resolve(id) {
        if (!id) return;
        swal({
            title: "Xác nhận xử lý",
            text: "Bạn muốn đánh dấu báo cáo này là đã xử lý?",
            icon: "warning",
            buttons: true
        }).then(function (ok) {
            if (!ok) return;
            $.post(config.urls.resolve, { id: id })
                .done(function (res) {
                    if (res.success) {
                        swal("Thành công!", res.message, "success").then(function () { table.ajax.reload(); });
                    } else {
                        swal("Lỗi!", res.message || 'Không thể xử lý', "error");
                    }
                })
                .fail(function () { swal("Lỗi!", "Không thể xử lý", "error"); });
        });
    }

    function reject(id) {
        if (!id) return;
        // open prompt to get reason
        swal({
            text: 'Nhập lý do từ chối:',
            content: "input",
            button: { text: "Xác nhận", closeModal: false }
        }).then(function (reason) {
            if (!reason) { swal("Lỗi!", "Vui lòng nhập lý do", "error"); return; }
            $.post(config.urls.reject, { id: id, lyDo: reason })
                .done(function (res) {
                    if (res.success) {
                        swal("Thành công!", res.message, "success").then(function () { table.ajax.reload(); });
                    } else {
                        swal("Lỗi!", res.message || 'Không thể từ chối', "error");
                    }
                })
                .fail(function () { swal("Lỗi!", "Không thể từ chối", "error"); });
        });
    }

    function remove(id) {
        if (!id) return;
        swal({
            title: "Xác nhận xóa",
            text: "Báo cáo sẽ bị xóa vĩnh viễn.",
            icon: "warning",
            buttons: true,
            dangerMode: true
        }).then(function (ok) {
            if (!ok) return;
            $.post(config.urls.delete, { id: id })
                .done(function (res) {
                    if (res.success) {
                        swal("Đã xóa", res.message, "success").then(function () { table.ajax.reload(); });
                    } else {
                        swal("Lỗi!", res.message || 'Không thể xóa', "error");
                    }
                })
                .fail(function () { swal("Lỗi!", "Không thể xóa", "error"); });
        });
    }

    // helpers
    function getStatusBadge(status) {
        switch ((status || '').toString()) {
            case 'CHO_XU_LY': return '<span class="badge badge-warning">Chờ xử lý</span>';
            case 'DANG_XU_LY': return '<span class="badge badge-info">Đang xử lý</span>';
            case 'DA_XU_LY': return '<span class="badge badge-success">Đã xử lý</span>';
            case 'TU_CHOI': return '<span class="badge badge-danger">Từ chối</span>';
            default: return '<span class="badge badge-secondary">' + escapeHtml(status || 'N/A') + '</span>';
        }
    }
    function getTypeBadge(type) {
        switch ((type || '').toString()) {
            case 'PHONG': return '<span class="badge badge-primary"><i class="fas fa-building"></i> Phòng</span>';
            case 'NGUOIDUNG': return '<span class="badge badge-primary"><i class="fas fa-user"></i> Người dùng</span>';
            case 'DANHGIA': return '<span class="badge badge-primary"><i class="fas fa-star"></i> Đánh giá</span>';
            default: return '<span class="badge badge-secondary">' + escapeHtml(type || 'N/A') + '</span>';
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

    // expose public API
    return {
        init: init,
        viewDetail: viewDetail,
        resolve: resolve,
        reject: reject,
        delete: remove
    };
})(jQuery);