/**
 * ========================================
 * SIDEBAR MANAGER
 * - Active Navigation State Handler
 * - Statistics & Counters
 * ========================================
 */

(function () {
    'use strict';

    // ========================================
    // NAVIGATION ACTIVE STATE
    // ========================================

    /**
     * Set active menu item based on current URL
     */
    function setActiveMenuItem() {
        // Include search so we can detect query strings (e.g. /Users?role=nguoithue)
        var currentPath = (window.location.pathname + window.location.search).toLowerCase();
        var currentController = getControllerFromPath(currentPath);
        var currentAction = getActionFromPath(currentPath);

        console.log('📍 Current Path:', currentPath);
        console.log('📍 Current Controller:', currentController);
        console.log('📍 Current Action:', currentAction);

        // Remove all active classes first
        $('.nav-item').removeClass('active submenu');
        $('.nav-collapse li').removeClass('active');
        $('.collapse').removeClass('show');
        
        console.log('🧹 Cleared all active classes');

        // Find and activate the matching nav item
        var $matchedItem = null;

        // Check submenu items first (more specific match)
        $('.nav-collapse a').each(function () {
            var $link = $(this);
            var href = $link.attr('href');

            console.log('🔍 Checking submenu link:', href);

            if (href && isPathMatch(href, currentPath, currentController, currentAction)) {
                // Found a submenu match - expand parent and highlight the submenu item
                var $collapseParent = $link.closest('.collapse');
                if ($collapseParent.length > 0) {
                    $collapseParent.addClass('show');

                    // Also set the collapse trigger to expanded if present
                    var collapseId = $collapseParent.attr('id');
                    if (collapseId) {
                        var $trigger = $('a[href="#' + collapseId + '"]');
                        $trigger.attr('aria-expanded', 'true');
                    }

                    console.log('✅ Expanded parent menu');
                    console.log('✅ Matched submenu item:', $link.text().trim());

                    // Highlight the submenu li
                    var $submenuLi = $link.closest('li');
                    if ($submenuLi.length > 0) {
                        $submenuLi.addClass('active');
                    }

                    // Highlight parent nav-item too (so higher-level menu is visibly active)
                    // Attempt common structures: collapse inside nav-item or collapse following a nav-item trigger
                    var $parentNavItem = $collapseParent.closest('.nav-item');
                    if ($parentNavItem.length > 0) {
                        $parentNavItem.addClass('active submenu');
                    } else {
                        // fallback: try to find previous nav-item (some templates use different structure)
                        var $prevNavItem = $collapseParent.prev('.nav-item, .nav-link, .nav-item > a');
                        if ($prevNavItem.length > 0) {
                            $prevNavItem.closest('.nav-item').addClass('active submenu');
                        }
                    }
                } else {
                    // No collapse parent (submenu might be rendered differently) - highlight link directly
                    $link.closest('li').addClass('active');
                    $link.closest('.nav-item').addClass('active submenu');
                }

                $matchedItem = $link;
                return false; // break loop
            }
        });

        // If no submenu match, check main nav items (only those WITHOUT submenus)
        if (!$matchedItem) {
            $('.nav-primary > .nav-item > a').each(function () {
                var $link = $(this);
                var href = $link.attr('href');

                console.log('🔍 Checking main nav link:', href);

                // Skip items with data-toggle (these are collapse triggers with submenus)
                if ($link.attr('data-toggle')) {
                    console.log('⏭️ Skipping collapse trigger (has submenu)');
                    return true; // continue
                }

                if (href && isPathMatch(href, currentPath, currentController, currentAction)) {
                    // This is a direct link without submenu - highlight it
                    $link.closest('.nav-item').addClass('active');
                    $matchedItem = $link;
                    console.log('✅ Matched main nav item (highlighted):', $link.find('p').text().trim());
                    return false; // break loop
                }
            });
        }

        // If still no match, check if we're on Dashboard
        if (!$matchedItem && (currentController === 'dashboard' || currentPath === '/' || currentPath === '' || currentPath === '/home')) {
            $('.nav-primary > .nav-item').first().addClass('active');
            console.log('✅ Set Dashboard as active (default)');
            $matchedItem = true;
        }

        // If no match found at all
        if (!$matchedItem && currentController !== 'dashboard') {
            console.warn('⚠️ No matching nav item found for:', currentPath);
        }
        
        console.log('✅ setActiveMenuItem completed');
    }

    /**
     * Check if href matches current path
     */
    function isPathMatch(href, currentPath, currentController, currentAction) {
        if (!href) return false;

        var normalizedHref = href.toLowerCase().trim();

        // Ignore in-page anchors used as collapse triggers
        if (normalizedHref.startsWith('#')) {
            return false;
        }

        // If href is absolute (contains protocol/host), parse it
        var hrefPathWithQuery = normalizedHref;
        try {
            var parser = document.createElement('a');
            parser.href = href;
            // If parser.pathname is available it gives a normalized pathname; include search too
            if (parser.pathname) {
                hrefPathWithQuery = (parser.pathname + (parser.search || '')).toLowerCase();
            }
        } catch (e) {
            // ignore parse errors and use normalizedHref
        }

        // Exact match including query
        if (hrefPathWithQuery === currentPath) {
            console.log('   ✅ Exact match:', href);
            return true;
        }

        // Compare path components (ignore query for controller/action match)
        var hrefPathOnly = hrefPathWithQuery.split('?')[0];
        var currentPathOnly = currentPath.split('?')[0];

        // If path-only exact match
        if (hrefPathOnly === currentPathOnly) {
            console.log('   ✅ Path-only exact match (ignoring query):', href);
            return true;
        }

        // Extract components
        var hrefController = getControllerFromPath(hrefPathWithQuery);
        var hrefAction = getActionFromPath(hrefPathWithQuery);

        console.log('   🔍 Comparing:', {
            hrefController: hrefController,
            currentController: currentController,
            hrefAction: hrefAction,
            currentAction: currentAction
        });

        // Controller must match
        if (hrefController !== currentController) {
            console.log('   ❌ Controller mismatch');
            return false;
        }

        // If controller matches, check action (allow index/empty equivalence)
        var actionMatches = (
            hrefAction === currentAction ||
            (hrefAction === 'index' && (currentAction === '' || currentAction === 'index')) ||
            (hrefAction === '' && (currentAction === 'index' || currentAction === ''))
        );

        if (!actionMatches) {
            console.log('   ❌ Action mismatch');
            return false;
        }

        // If we're here, controller and action match
        // Now consider query strings
        var hrefHasQuery = hrefPathWithQuery.indexOf('?') > -1;
        var currentHasQuery = currentPath.indexOf('?') > -1;

        if (hrefHasQuery && currentHasQuery) {
            // Both have query strings - check if they match or href params are a subset
            var hrefQuery = hrefPathWithQuery.substring(hrefPathWithQuery.indexOf('?'));
            var currentQuery = currentPath.substring(currentPath.indexOf('?'));
            
            console.log('   🔍 Query comparison:', {
                hrefQuery: hrefQuery,
                currentQuery: currentQuery
            });
            
            if (hrefQuery === currentQuery) {
                console.log('   ✅ Query string exact match');
                return true;
            }
            
            // Partial query match - check if href query params are in current
            var hrefParams = parseQueryString(hrefQuery);
            var currentParams = parseQueryString(currentQuery);
            
            var allHrefParamsMatch = true;
            for (var key in hrefParams) {
                if (hrefParams.hasOwnProperty(key) && hrefParams[key] !== currentParams[key]) {
                    allHrefParamsMatch = false;
                    break;
                }
            }
            
            if (allHrefParamsMatch) {
                console.log('   ✅ Query params match (href subset of current)');
                return true;
            }
            
            console.log('   ❌ Query params mismatch');
            return false;
        }
        
        if (hrefHasQuery && !currentHasQuery) {
            // Allow match when href has filtering query but current url is the controller/action root.
            // This enables highlighting parent menu when the submenu is a filtered view like /Users?role=nguoithue
            console.log('   ⚠️ href has query, current does not - allowing match based on controller/action');
            return true;
        }
        
        if (!hrefHasQuery && currentHasQuery) {
            // Current has query but href doesn't - still consider it a match if controller/action match
            console.log('   ⚠️ href has no query, but current does - allowing match based on controller/action');
            return true;
        }

        // Neither has query string - controller and action match is enough
        console.log('   ✅ Controller and action match (no query strings)');
        return true;
    }

    /**
     * Parse query string into object
     */
    function parseQueryString(queryString) {
        var params = {};
        if (!queryString || queryString === '?') return params;
        
        // Remove leading '?'
        var query = queryString.charAt(0) === '?' ? queryString.substring(1) : queryString;
        var pairs = query.split('&');
        
        for (var i = 0; i < pairs.length; i++) {
            if (!pairs[i]) continue;
            var pair = pairs[i].split('=');
            var key = decodeURIComponent(pair[0] || '').toLowerCase();
            var value = pair.length > 1 ? decodeURIComponent(pair[1] || '') : '';
            params[key] = value;
        }
        
        return params;
    }

    /**
     * Extract controller name from path
     */
    function getControllerFromPath(path) {
        // Remove query string first
        var pathWithoutQuery = (path || '').split('?')[0];
        
        // Split by '/' and filter empty parts
        var parts = pathWithoutQuery.split('/').filter(function (p) { return p.length > 0; });
        
        // ASP.NET MVC format: /Controller/Action or /Controller/Action/Id
        if (parts.length > 0) {
            return parts[0].toLowerCase();
        }
        
        return 'dashboard'; // default
    }

    /**
     * Extract action name from path
     */
    function getActionFromPath(path) {
        // Remove query string first
        var pathWithoutQuery = (path || '').split('?')[0];
        
        // Split by '/' and filter empty parts
        var parts = pathWithoutQuery.split('/').filter(function (p) { return p.length > 0; });
        
        // ASP.NET MVC format: /Controller/Action or /Controller/Action/Id
        if (parts.length > 1) {
            return parts[1].toLowerCase();
        }
        
        return 'index'; // default action
    }

    /**
     * Handle submenu clicks
     */
    function initializeSubmenuToggle() {
        // Keep existing Atlantis behavior but ensure it works with our active state
        $('[data-toggle="collapse"]').on('click', function (e) {
            var $this = $(this);
            var target = $this.attr('href');

            if (target && target.startsWith('#')) {
                // Let Bootstrap handle the collapse
                return;
            }
        });
    }

    /**
     * Highlight parent nav item when submenu is active
     */
    function highlightParentWhenSubmenuActive() {
        // Ensure parent menus with open submenus are highlighted (fallback)
        $('.collapse.show').each(function () {
            var $collapse = $(this);
            var collapseId = $collapse.attr('id');

            // try to set the trigger aria-expanded and add active class to the trigger nav-item
            if (collapseId) {
                var $trigger = $('a[href="#' + collapseId + '"]');
                if ($trigger.length > 0) {
                    $trigger.attr('aria-expanded', 'true');
                    $trigger.closest('.nav-item').addClass('active submenu');
                }
            }

            var $parentNavItem = $collapse.closest('.nav-item');
            if ($parentNavItem.length > 0 && !$parentNavItem.hasClass('active')) {
                $parentNavItem.addClass('active submenu');
            }
        });
    }

    // ========================================
    // STATISTICS & COUNTERS
    // ========================================

    /**
     * Load user statistics
     */
    function loadUserStatistics() {
        console.log('📊 Loading user statistics for sidebar...');

        $.ajax({
            url: '/Users/GetUserStatistics',
            type: 'GET',
            dataType: 'json',
            timeout: 5000,
            success: function (response) {
                console.log('✅ User Statistics Response:', response);

                if (response.success && response.data) {
                    var stats = response.data;

                    // Update counters with different colors
                    $('#tenantCount')
                        .text(stats.TotalTenants || 0)
                        .removeClass('badge-secondary')
                        .addClass('badge-primary');

                    $('#landlordCount')
                        .text(stats.TotalLandlords || 0)
                        .removeClass('badge-secondary')
                        .addClass('badge-info');

                    $('#adminCount')
                        .text(stats.TotalAdmins || 0)
                        .removeClass('badge-secondary')
                        .addClass('badge-danger');

                    console.log('✅ User statistics updated successfully:', stats);
                } else {
                    console.warn('⚠️ Invalid response format:', response);
                }
            },
            error: function (xhr, status, error) {
                console.error('❌ User statistics error:', error);
                console.error('Status:', status);
                console.error('Response:', xhr.responseText);
            }
        });
    }

    /**
     * Load host statistics
     */
    function loadHostStatistics() {
        console.log('📊 Loading host statistics for sidebar...');

        $.ajax({
            url: '/Hosts/GetHostStatistics',
            type: 'GET',
            dataType: 'json',
            timeout: 5000,
            success: function (response) {
                console.log('✅ Host Statistics Response:', response);

                if (response.success && response.data) {
                    var stats = response.data;

                    console.log('📈 Host stats data:', {
                        pending: stats.PendingCount,
                        approved: stats.ApprovedCount,
                        rejected: stats.RejectedCount,
                        total: stats.TotalCount
                    });

                    // Update counters with different colors
                    var pendingElement = $('#hostPendingCount');
                    var approvedElement = $('#hostApprovedCount');
                    var rejectedElement = $('#hostRejectedCount');

                    console.log('🔍 Badge elements found:', {
                        pending: pendingElement.length,
                        approved: approvedElement.length,
                        rejected: rejectedElement.length
                    });

                    pendingElement
                        .text(stats.PendingCount || 0)
                        .removeClass('badge-secondary')
                        .addClass('badge-warning');

                    approvedElement
                        .text(stats.ApprovedCount || 0)
                        .removeClass('badge-secondary')
                        .addClass('badge-success');

                    rejectedElement
                        .text(stats.RejectedCount || 0)
                        .removeClass('badge-secondary')
                        .addClass('badge-danger');

                    console.log('✅ Host statistics updated successfully');
                } else {
                    console.warn('⚠️ Invalid response format:', response);
                }
            },
            error: function (xhr, status, error) {
                console.error('❌ Host statistics error:', error);
                console.error('Status:', status);
                console.error('Response:', xhr.responseText);
            }
        });
    }

    /**
     * Load report statistics
     */
    function loadReportStatistics() {
        console.log('📊 Loading report statistics for sidebar...');

        $.ajax({
            url: '/Reports/GetStatistics',
            type: 'GET',
            dataType: 'json',
            timeout: 5000,
            success: function (response) {
                console.log('✅ Report Statistics Response:', response);

                if (response.success && response.data) {
                    var stats = response.data;

                    console.log('📈 Report stats data:', {
                        pending: stats.PendingReports,
                        total: stats.TotalReports
                    });

                    // Update report counter with badge color
                    var reportElement = $('#reportPendingCount');
                    if (reportElement.length > 0) {
                        reportElement
                            .text(stats.PendingReports || 0)
                            .removeClass('badge-secondary')
                            .addClass('badge-danger');
                        
                        console.log('✅ Report statistics updated successfully');
                    } else {
                        console.warn('⚠️ Report counter element not found');
                    }
                } else {
                    console.warn('⚠️ Invalid report response format:', response);
                }
            },
            error: function (xhr, status, error) {
                console.error('❌ Report statistics error:', error);
                console.error('Status:', status);
                console.error('Response:', xhr.responseText);
            }
        });
    }

    /**
     * Load support statistics
     */
    function loadSupportStatistics() {
        console.log('📊 Loading support statistics for sidebar...');

        $.ajax({
            url: '/Support/GetStatistics',
            type: 'GET',
            dataType: 'json',
            timeout: 5000,
            success: function (response) {
                console.log('✅ Support Statistics Response:', response);

                if (response.success && response.data) {
                    var stats = response.data;

                    console.log('📈 Support stats data:', {
                        new: stats.NewRequests,
                        processing: stats.ProcessingRequests,
                        total: stats.TotalRequests
                    });

                    // Update support counter with badge color
                    var supportElement = $('#supportPendingCount');
                    if (supportElement.length > 0) {
                        var pendingCount = (stats.NewRequests || 0) + (stats.ProcessingRequests || 0);
                        supportElement
                            .text(pendingCount)
                            .removeClass('badge-secondary')
                            .addClass('badge-info');
                        
                        console.log('✅ Support statistics updated successfully');
                    } else {
                        console.warn('⚠️ Support counter element not found');
                    }
                } else {
                    console.warn('⚠️ Invalid support response format:', response);
                }
            },
            error: function (xhr, status, error) {
                console.error('❌ Support statistics error:', error);
                console.error('Status:', status);
                console.error('Response:', xhr.responseText);
            }
        });
    }

    // ========================================
    // INITIALIZATION
    // ========================================

    /**
     * Initialize sidebar manager
     */
    function init() {
        console.log('🚀 Sidebar Manager - Initializing...');

        // Initialize navigation active state
        console.log('🔧 Initializing navigation active state...');
        setActiveMenuItem();
        initializeSubmenuToggle();
        highlightParentWhenSubmenuActive();
        console.log('✅ Navigation active state initialized');

        // Load statistics
        console.log('📊 Loading statistics...');
        loadUserStatistics();
        loadHostStatistics();
        loadReportStatistics();
        loadSupportStatistics();

        // Refresh statistics every 2 minutes
        setInterval(function () {
            console.log('🔄 Refreshing statistics...');
            loadUserStatistics();
            loadHostStatistics();
            loadReportStatistics();
            loadSupportStatistics();
        }, 120000);

        console.log('✅ Sidebar Manager Initialized Successfully');
    }

    // Wait for DOM and jQuery to be ready
    if (typeof jQuery === 'undefined') {
        console.error('❌ jQuery is not loaded. Sidebar manager will not work properly.');
    } else {
        $(document).ready(function () {
            init();
        });
    }

    // Expose for manual refresh if needed
    window.SidebarManager = {
        refreshNavigation: setActiveMenuItem,
        refreshStatistics: function () {
            loadUserStatistics();
            loadHostStatistics();
            loadReportStatistics();
            loadSupportStatistics();
        }
    };

})();
