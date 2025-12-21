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
            start: '',
            resolve: '',
            reject: '',
            delete: '',
            getStatistics: ''
        }
    };

    function init(options) {
        console.log('🚀 ReportsPage.init() called with options:', options);
        config = $.extend(true, config, options || {});
        console.log('✅ Config updated:', config);
        
        initDataTable();
        initFilters();
        initModals();
        
        // Load stats AFTER table is ready (brief delay to ensure DOM is ready)
        setTimeout(function () {
            console.log('📊 Loading initial statistics...');
            loadStatistics(getCurrentFilters());
        }, 500);
    }

    function initIndex(options) {
        init(options);
    }

    function getCurrentFilters() {
        var filters = {
            search: $('#searchInput').val() || '',
            status: $('#statusFilter').val() || '',
            loaiThucThe: $('#typeFilter').val() || ''
        };
        console.log('🔍 getCurrentFilters():', filters);
        return filters;
    }

    function initDataTable() {
        console.log('🔧 Initializing DataTable...');
        
        table = $('#reportsTable').DataTable({
            processing: true,
            serverSide: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            order: [[3, 'desc']],
            dom: '<"top">rt<"bottom"ip><"clear">',
            ajax: {
                url: config.urls.getReports,
                type: 'POST',
                data: function (d) {
                    // IMPORTANT: match controller signature
                    // GetReports(int draw,int start,int length,string search="",string status="",string loaiThucThe="")
                    var f = getCurrentFilters();
                    d.search = f.search;
                    d.status = f.status;
                    d.loaiThucThe = f.loaiThucThe;
                    
                    console.log('📤 DataTable sending to server:', {
                        draw: d.draw,
                        start: d.start,
                        length: d.length,
                        search: d.search,
                        status: d.status,
                        loaiThucThe: d.loaiThucLe
                    });
                },
                dataSrc: function (json) {
                    console.log('📥 DataTable received response:', json);
                    if (!json) return [];
                    if (Array.isArray(json.data)) return json.data;
                    if (Array.isArray(json.items)) return json.items;
                    if (json.Items && Array.isArray(json.Items)) return json.Items;
                    return [];
                },
                error: function (xhr, status, error) {
                    console.error('❌ DataTable error:', { xhr: xhr, status: status, error: error });
                    showNotification('error', 'Lỗi', 'Không thể tải dữ liệu báo cáo');
                }
            },
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/vi.json',
                processing: '<div class="spinner-border text-primary" role="status"><span class="sr-only">Đang tải...</span></div>',
                emptyTable: 'Không có báo cáo'
            },
            columns: [
                { data: null },
                { data: 'LoaiThucThe' },
                { data: 'TieuDe' },
                { data: 'ThoiGianBaoCao' },
                { data: 'TrangThai' },
                { data: 'KetQua' },
                { data: 'BaoCaoId' }
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

                        // Handle multiple date formats: ISO string, /Date(ticks)/, or already formatted
                        var d;
                        
                        // Try to parse /Date(ticks)/ format
                        var dateMatch = String(data).match(/\/Date\((\d+)\)/);
                        if (dateMatch) {
                            d = new Date(parseInt(dateMatch[1]));
                        } else {
                            // Try standard date parsing
                            d = new Date(data);
                        }
                        
                        if (!isNaN(d.getTime())) {
                            return pad(d.getDate()) + '/' + pad(d.getMonth() + 1) + '/' + d.getFullYear() +
                                ' <br/><small class="text-muted">' + pad(d.getHours()) + ':' + pad(d.getMinutes()) + '</small>';
                        }
                        return escapeHtml(data);
                    }
                },
                {
                    targets: 4,
                    render: function (data) {
                        return getStatusBadge(data);
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
                        var html = '<div class="btn-group btn-group-sm" role="group">';
                        
                        // Normalize status for comparison
                        var st = (row.TrangThai || '').toString().trim().toUpperCase();
                        // Remove spaces and underscores for comparison
                        st = st.replace(/[\s_]/g, '');
                        
                        console.log('🎯 Rendering actions for report:', {
                            id: id,
                            originalStatus: row.TrangThai,
                            normalizedStatus: st
                        });
                        
                        // Xem chi tiết - luôn có
                        html += '<button class="btn btn-info" onclick="viewReportDetails(\'' + id + '\')" title="Xem chi tiết"><i class="fa fa-eye"></i></button>';
                        
                        // Bắt đầu xử lý - chỉ khi CHO_XU_LY
                        if (st === 'CHOXULY' || st === 'CHO_XU_LY') {
                            console.log('   ➕ Adding START button');
                            html += '<button class="btn btn-primary" onclick="startReport(\'' + id + '\')" title="Bắt đầu xử lý"><i class="fa fa-play"></i></button>';
                        }
                        
                        // Xử lý hoàn thành - khi CHO_XU_LY hoặc DANG_XU_LY
                        if (st === 'CHOXULY' || st === 'DANGXULY' || st === 'CHO_XU_LY' || st === 'DANG_XU_LY') {
                            console.log('   ➕ Adding RESOLVE & REJECT buttons');
                            html += '<button class="btn btn-success" onclick="resolveReport(\'' + id + '\')" title="Xử lý"><i class="fa fa-check"></i></button>';
                            html += '<button class="btn btn-warning" onclick="rejectReport(\'' + id + '\')" title="Từ chối"><i class="fa fa-ban"></i></button>';
                        }

                        // Xóa - luôn có
                        html += '<button class="btn btn-danger" onclick="deleteReport(\'' + id + '\')" title="Xóa"><i class="fa fa-trash"></i></button>';
                        
                        html += '</div>';
                        console.log('   Final HTML:', html);
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
        
        $('#btnSearchReports').on('click', function () {
            console.log('🔘 Search button clicked');
            reload();
        });

        $('#searchInput').on('keyup', function (e) {
            if (e.keyCode === 13) {
                console.log('⌨️ Enter pressed in search');
                reload();
            }
        });

        $('#statusFilter, #typeFilter').on('change', function () {
            console.log('🔘 Filter dropdown changed:', $(this).attr('id'));
            reload();
        });
        
        console.log('✅ Filter handlers initialized');
    }

    function initModals() {
        $('#reportDetailModal').on('hidden.bs.modal', function () {
            $('#reportDetailContent').html('');
        });
    }

    function reload() {
        console.log('🔄 reload() called - reloading table and stats');
        
        if (table) {
            // CRITICAL: Use proper DataTable API to reload
            table.ajax.reload(function (json) {
                console.log('✅ Table reloaded with response:', json);
                // After table reloads, refresh statistics
                loadStatistics(getCurrentFilters());
            }, false); // false = don't reset paging
        } else {
            console.warn('⚠️ DataTable not initialized yet');
        }
        
        // Also refresh sidebar stats
        if (window.SidebarManager && typeof window.SidebarManager.refreshStatistics === 'function') {
            console.log('🔄 Refreshing sidebar statistics');
            window.SidebarManager.refreshStatistics();
        }
    }

    function clearFilters() {
        console.log('🗑️ Clearing all filters');
        $('#statusFilter').val('');
        $('#typeFilter').val('');
        $('#searchInput').val('');
        reload();
    }

    function refreshTable() {
        console.log('🔄 Refresh table button clicked');
        reload();
    }

    // Public action helpers
    function viewDetail(id) {
        console.log('👁️ Viewing report detail:', id);
        if (!id) return;
        $('#reportDetailModal').modal('show');
        $('#reportDetailContent').html('<div class="text-center py-5"><div class="spinner-border text-primary"></div><p class="mt-2">Đang tải...</p></div>');
        $.get(config.urls.getReportDetail, { id: id })
            .done(function (res) {
                console.log('✅ Report detail loaded:', res);
                if (res && res.success) buildDetail(res.data);
                else $('#reportDetailContent').html('<div class="alert alert-danger">' + escapeHtml((res && res.message) ? res.message : 'Lỗi') + '</div>');
            })
            .fail(function (err) {
                console.error('❌ Failed to load report detail:', err);
                $('#reportDetailContent').html('<div class="alert alert-danger">Lỗi khi tải chi tiết</div>');
            });
    }

    function buildDetail(data) {
        var status = getStatusBadge(data.TrangThai);
        var html = '<div class="row"><div class="col-md-6"><label class="text-muted">Mã báo cáo</label><p class="font-weight-bold">#' + (data.SoBaoCao || '') + '</p></div>';
        html += '<div class="col-md-6"><label class="text-muted">Trạng thái</label><p>' + status + '</p></div></div>';
        html += '<hr><div class="form-group"><label class="text-muted">Tiêu đề</label><p class="font-weight-bold">' + escapeHtml(data.TieuDe || '') + '</p></div>';
        html += '<div class="form-group"><label class="text-muted">Mô tả</label><p>' + escapeHtml(data.MoTa || '-') + '</p></div>';
        if (data.KetQua) html += '<div class="form-group"><label class="text-muted">Kết quả</label><p>' + escapeHtml(data.KetQua) + '</p></div>';
        $('#reportDetailContent').html(html);

        // Make modal footer buttons actionable
        var id = data.BaoCaoId;
        var st = (data.TrangThai || '').toString();

        // Resolve/Reject only when pending/processing
        if (st === 'CHO_XU_LY' || st === 'DANG_XU_LY' || st === '') {
            $('#modalResolveBtn').show().off('click').on('click', function () {
                $('#reportDetailModal').modal('hide');
                resolve(id);
            });
            $('#modalRejectBtn').show().off('click').on('click', function () {
                $('#reportDetailModal').modal('hide');
                reject(id);
            });
        } else {
            $('#modalResolveBtn, #modalRejectBtn').hide();
        }

        // Add delete button in modal footer if not exists
        if ($('#modalDeleteBtn').length === 0) {
            var $btn = $('<button/>', {
                id: 'modalDeleteBtn',
                type: 'button',
                class: 'btn btn-danger',
                html: '<i class="fa fa-trash"></i> Xóa'
            });
            $('.modal-footer', '#reportDetailModal').append($btn);
        }

        $('#modalDeleteBtn').off('click').on('click', function () {
            $('#reportDetailModal').modal('hide');
            remove(id);
        });
    }

    function resolve(id) {
        console.log('✅ Resolving report:', id);
        if (!id) return;
        
        // Open resolve modal to get result details
        $('#resolveReportId').val(id);
        $('#resolveResult').val('');
        $('#resolveModal').modal('show');
    }

    function start(id) {
        console.log('🚀 Starting to process report:', id);
        if (!id) return;
        swal({
            title: "Bắt đầu xử lý?",
            text: "Báo cáo sẽ được chuyển sang trạng thái 'Đang xử lý'",
            icon: "info",
            buttons: true
        }).then(function (ok) {
            if (!ok) return;
            
            // Call API to update status to DANG_XU_LY
            $.ajax({
                url: config.urls.start,
                type: 'POST',
                data: { id: id },
                dataType: 'json',
                success: function (res) {
                    console.log('✅ Report started:', res);
                    if (res && res.success) {
                        swal("Thành công!", res.message || 'Đã bắt đầu xử lý', "success").then(function () {
                            reload();
                        });
                    } else {
                        swal("Lỗi!", (res && res.message) ? res.message : 'Không thể bắt đầu xử lý', "error");
                    }
                },
                error: function (err) {
                    console.error('❌ Failed to start:', err);
                    swal("Lỗi!", "Không thể bắt đầu xử lý", "error");
                }
            });
        });
    }

    function reject(id) {
        console.log('🚫 Rejecting report:', id);
        if (!id) return;
        
        // Open reject modal
        $('#rejectReportId').val(id);
        $('#rejectReason').val('');
        $('#rejectModal').modal('show');
    }

    function remove(id) {
        console.log('🗑️ Deleting report:', id);
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
                    console.log('✅ Report deleted:', res);
                    if (res && res.success) {
                        swal("Đã xóa", res.message || 'Xóa thành công', "success").then(function () { reload(); });
                    } else {
                        swal("Lỗi!", (res && res.message) ? res.message : 'Không thể xóa', "error");
                    }
                })
                .fail(function (err) {
                    console.error('❌ Failed to delete:', err);
                    swal("Lỗi!", "Không thể xóa", "error");
                });
        });
    }

    function submitResolve() {
        console.log('📤 Submitting resolve...');
        var id = $('#resolveReportId').val();
        var result = $('#resolveResult').val();

        if (!id) {
            swal("Lỗi!", "ID báo cáo không hợp lệ", "error");
            return;
        }

        if (!result || !result.trim()) {
            swal("Lỗi!", "Vui lòng nhập kết quả xử lý", "error");
            return;
        }

        $.ajax({
            url: config.urls.resolve,
            type: 'POST',
            data: { id: id, ketQua: result.trim() },
            dataType: 'json',
            success: function (res) {
                console.log('✅ Report resolved:', res);
                if (res && res.success) {
                    $('#resolveModal').modal('hide');
                    swal("Thành công!", res.message || 'Đã xử lý', "success").then(function () {
                        reload();
                    });
                } else {
                    swal("Lỗi!", (res && res.message) ? res.message : 'Không thể xử lý', "error");
                }
            },
            error: function (err) {
                console.error('❌ Failed to resolve:', err);
                swal("Lỗi!", "Không thể xử lý", "error");
            }
        });
    }

    function submitReject() {
        console.log('📤 Submitting reject...');
        var id = $('#rejectReportId').val();
        var reason = $('#rejectReason').val();

        if (!id) {
            swal("Lỗi!", "ID báo cáo không hợp lệ", "error");
            return;
        }

        if (!reason || !reason.trim()) {
            swal("Lỗi!", "Vui lòng nhập lý do từ chối", "error");
            return;
        }

        $.ajax({
            url: config.urls.reject,
            type: 'POST',
            data: { id: id, lyDo: reason.trim() },
            dataType: 'json',
            success: function (res) {
                console.log('✅ Report rejected:', res);
                if (res && res.success) {
                    $('#rejectModal').modal('hide');
                    swal("Thành công!", res.message || 'Đã từ chối', "success").then(function () {
                        reload();
                    });
                } else {
                    swal("Lỗi!", (res && res.message) ? res.message : 'Không thể từ chối', "error");
                }
            },
            error: function (err) {
                console.error('❌ Failed to reject:', err);
                swal("Lỗi!", "Không thể từ chối", "error");
            }
        });
    }

    function loadStatistics(filters) {
        if (!config.urls.getStatistics) {
            console.warn('⚠️ getStatistics URL not configured');
            return;
        }

        var data = filters || {};
        console.log('📊 Loading statistics with filters:', data);

        $.ajax({
            url: config.urls.getStatistics,
            type: 'GET',
            data: data,
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
        var pendingElement = document.querySelector('[data-stat="pending"]');
        var processingElement = document.querySelector('[data-stat="processing"]');
        var resolvedElement = document.querySelector('[data-stat="resolved"]');
        var rejectedElement = document.querySelector('[data-stat="rejected"]');

        if (totalElement) {
            console.log('📊 Updating total:', stats.TotalReports);
            totalElement.textContent = stats.TotalReports || 0;
        }
        if (pendingElement) {
            console.log('⏳ Updating pending:', stats.PendingReports);
            pendingElement.textContent = stats.PendingReports || 0;
        }
        if (processingElement) {
            console.log('⚙️ Updating processing:', stats.ProcessingReports);
            processingElement.textContent = stats.ProcessingReports || 0;
        }
        if (resolvedElement) {
            console.log('✅ Updating resolved:', stats.ResolvedReports);
            resolvedElement.textContent = stats.ResolvedReports || 0;
        }
        if (rejectedElement) {
            console.log('❌ Updating rejected:', stats.RejectedReports);
            rejectedElement.textContent = stats.RejectedReports || 0;
        }
    }

    function showNotification(type, title, message) {
        if (typeof swal !== 'undefined') {
            swal(title, message, type);
        } else {
            alert(title + ': ' + message);
        }
    }

    // helpers
    function getStatusBadge(status) {
        var st = (status || '').toString().trim().toUpperCase();
        console.log('🔍 getStatusBadge converting:', status, '→', st);
        
        // Normalize status values (handle both with/without spacing)
        if (st.includes('CHO') || st === 'CHO_XU_LY') {
            return '<span class="badge badge-warning">Chờ xử lý</span>';
        }
        if (st.includes('DANG') || st === 'DANG_XU_LY' || st === 'DANGXULY') {
            return '<span class="badge badge-info">Đang xử lý</span>';
        }
        if (st.includes('DA') || st === 'DA_XU_LY' || st === 'DAXULY') {
            return '<span class="badge badge-success">Đã xử lý</span>';
        }
        if (st.includes('TU') || st === 'TU_CHOI' || st === 'TUCHOI') {
            return '<span class="badge badge-danger">Từ chối</span>';
        }
        return '<span class="badge badge-secondary">' + escapeHtml(status || 'N/A') + '</span>';
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
        start: start,
        resolve: resolve,
        reject: reject,
        delete: remove,
        submitResolve: submitResolve,
        submitReject: submitReject,
        loadStatistics: loadStatistics,
        reload: reload,
        clearFilters: clearFilters,
        refreshTable: refreshTable
    };
})(jQuery);

// Backward compat for inline onclicks in view
function clearFilters() { if (window.ReportsPage) window.ReportsPage.clearFilters(); }
function refreshTable() { if (window.ReportsPage) window.ReportsPage.refreshTable(); }

// These functions are referenced by `Views/Reports/Index.cshtml` inline onclick handlers
function viewReportDetails(id) { if (window.ReportsPage) window.ReportsPage.viewDetail(id); }
function startReport(id) { if (window.ReportsPage) window.ReportsPage.start(id); }
function resolveReport(id) { if (window.ReportsPage) window.ReportsPage.resolve(id); }
function rejectReport(id) { if (window.ReportsPage) window.ReportsPage.reject(id); }
function deleteReport(id) { if (window.ReportsPage) window.ReportsPage.delete(id); }
function submitResolve() { if (window.ReportsPage) window.ReportsPage.submitResolve(); }
function submitReject() { if (window.ReportsPage) window.ReportsPage.submitReject(); }

// Optional export hook (no-op until server endpoint implemented)
function exportReports() {
    console.log('📥 Export reports requested');
    // If you already have an export endpoint, wire it here.
    // Keeping it safe so clicking Export won't break other JS on the page.
    if (typeof swal !== 'undefined') {
        swal('Thông báo', 'Chức năng Export chưa được cấu hình.', 'info');
    }
}