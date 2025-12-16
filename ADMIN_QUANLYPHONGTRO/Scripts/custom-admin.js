/*!
 * Custom Admin JavaScript
 * TroTot Admin Panel
 * Version: 1.0.0
 */

(function($) {
    'use strict';

    $(document).ready(function() {
        console.log('? TroTot Admin - Custom JS Loaded!');

        // Initialize all components
        initSidebarNavigation();
        initSidebarToggle();
        initTooltips();
        initSmoothScroll();
        initDataTables();
    });

    // ===== SIDEBAR NAVIGATION =====
    function initSidebarNavigation() {
        $('.sidebar .nav-item a[data-toggle="collapse"]').on('click', function(e) {
            var target = $(this).attr('href');
            
            // Toggle active class
            $(this).parent().toggleClass('active');
            
            // Collapse other menus (optional, comment out if you want multiple menus open)
            // $('.sidebar .nav-item').not($(this).parent()).removeClass('active');
            // $('.sidebar .collapse').not(target).collapse('hide');
        });

        // Set active menu based on current URL
        var currentPath = window.location.pathname;
        $('.sidebar .nav-item a').each(function() {
            var href = $(this).attr('href');
            if (href && currentPath.indexOf(href) !== -1 && href !== '/' && href !== '#') {
                $(this).parent().addClass('active');
                $(this).parents('.collapse').collapse('show');
                $(this).parents('.nav-item').addClass('active');
            }
        });
    }

    // ===== TOGGLE SIDEBAR MINI =====
    function initSidebarToggle() {
        $('.toggle-sidebar').on('click', function() {
            $('body').toggleClass('sidebar-mini');
            
            // Save state to localStorage
            var isMini = $('body').hasClass('sidebar-mini');
            localStorage.setItem('sidebarMini', isMini);
        });

        // Restore sidebar state from localStorage
        var isMini = localStorage.getItem('sidebarMini') === 'true';
        if (isMini) {
            $('body').addClass('sidebar-mini');
        }
    }

    // ===== TOOLTIPS =====
    function initTooltips() {
        if (typeof $.fn.tooltip !== 'undefined') {
            $('[data-toggle="tooltip"]').tooltip();
        }
    }

    // ===== SMOOTH SCROLL =====
    function initSmoothScroll() {
        $('a[href*="#"]:not([href="#"])').on('click', function(e) {
            if (location.pathname.replace(/^\//, '') === this.pathname.replace(/^\//, '') 
                && location.hostname === this.hostname) {
                var target = $(this.hash);
                target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
                if (target.length) {
                    e.preventDefault();
                    $('html, body').animate({
                        scrollTop: target.offset().top - 70
                    }, 500);
                }
            }
        });
    }

    // ===== DATA TABLES =====
    function initDataTables() {
        if (typeof $.fn.DataTable !== 'undefined') {
            $('.datatable').DataTable({
                "language": {
                    "url": "//cdn.datatables.net/plug-ins/1.13.4/i18n/vi.json"
                },
                "pageLength": 10,
                "ordering": true,
                "searching": true
            });
        }
    }

    // ===== GLOBAL HELPER FUNCTIONS =====

    // Confirm Delete Dialog
    window.confirmDelete = function(message) {
        return confirm(message || 'B?n có ch?c ch?n mu?n xóa?');
    };

    // Show Loading Overlay
    window.showLoading = function(message) {
        message = message || '?ang x? lý...';
        if ($('.loading-overlay').length === 0) {
            $('body').append(
                '<div class="loading-overlay">' +
                '<div class="loading-content">' +
                '<div class="spinner-border text-primary" role="status"></div>' +
                '<p class="mt-2">' + message + '</p>' +
                '</div>' +
                '</div>'
            );
        }
    };

    // Hide Loading Overlay
    window.hideLoading = function() {
        $('.loading-overlay').fadeOut(300, function() {
            $(this).remove();
        });
    };

    // Show Toast Notification
    window.showToast = function(message, type) {
        type = type || 'info';
        var bgClass = 'bg-' + type;
        var iconClass = 'fa-info-circle';
        
        switch(type) {
            case 'success':
                iconClass = 'fa-check-circle';
                break;
            case 'danger':
            case 'error':
                iconClass = 'fa-exclamation-circle';
                bgClass = 'bg-danger';
                break;
            case 'warning':
                iconClass = 'fa-exclamation-triangle';
                break;
        }
        
        var toast = $('<div class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-delay="3000">' +
            '<div class="toast-header ' + bgClass + ' text-white">' +
            '<i class="fas ' + iconClass + ' mr-2"></i>' +
            '<strong class="mr-auto">Thông báo</strong>' +
            '<button type="button" class="ml-2 mb-1 close text-white" data-dismiss="toast" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '</div>' +
            '<div class="toast-body">' + message + '</div>' +
            '</div>');
        
        // Create toast container if not exists
        if ($('.toast-container').length === 0) {
            $('body').append('<div class="toast-container" style="position: fixed; top: 70px; right: 20px; z-index: 9999;"></div>');
        }
        
        $('.toast-container').append(toast);
        toast.toast('show');
        
        toast.on('hidden.bs.toast', function() {
            $(this).remove();
        });
    };

    // Format Number (Vietnamese)
    window.formatNumber = function(num) {
        return new Intl.NumberFormat('vi-VN').format(num);
    };

    // Format Currency (Vietnamese Dong)
    window.formatCurrency = function(num) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(num);
    };

    // Debounce Function
    window.debounce = function(func, wait) {
        var timeout;
        return function() {
            var context = this, args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(function() {
                func.apply(context, args);
            }, wait);
        };
    };

})(jQuery);

// ===== LOADING OVERLAY STYLES =====
// Add styles dynamically
$(function() {
    if ($('#custom-admin-styles').length === 0) {
        $('<style id="custom-admin-styles">')
            .text(`
                .loading-overlay {
                    position: fixed;
                    top: 0;
                    left: 0;
                    width: 100%;
                    height: 100%;
                    background: rgba(0, 0, 0, 0.5);
                    z-index: 9999;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }
                .loading-content {
                    text-align: center;
                    color: #fff;
                }
                .loading-content .spinner-border {
                    width: 3rem;
                    height: 3rem;
                }
            `)
            .appendTo('head');
    }
});
