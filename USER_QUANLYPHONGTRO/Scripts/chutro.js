// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    initializeSidebar();
    initializeSubmenu();
    initializeFilters();
    initializeInteractiveElements();
});

/**
 * Initialize sidebar toggle functionality
 */
function initializeSidebar() {
    const sidebarToggle = document.querySelector('.sidebar-toggle');
    const sidebar = document.getElementById('sidebar');
    
    if (!sidebarToggle || !sidebar) return;

    sidebarToggle.addEventListener('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        sidebar.classList.toggle('open');
    });

    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function (event) {
        const isClickInsideSidebar = sidebar.contains(event.target);
        const isClickOnToggle = sidebarToggle.contains(event.target);
        
        if (!isClickInsideSidebar && !isClickOnToggle) {
            sidebar.classList.remove('open');
        }
    });

    // Close sidebar when clicking on nav items
    const navItems = sidebar.querySelectorAll('.nav-item:not(.has-submenu)');
    navItems.forEach(function (item) {
        item.addEventListener('click', function () {
            // Only close on mobile
            if (window.innerWidth <= 768) {
                sidebar.classList.remove('open');
            }
        });
    });
}

/**
 * Initialize submenu toggle functionality
 */
function initializeSubmenu() {
    const submenuItems = document.querySelectorAll('.nav-item.has-submenu');
    
    submenuItems.forEach(function (item) {
        item.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            
            // Close other submenus
            submenuItems.forEach(function (otherItem) {
                if (otherItem !== item) {
                    otherItem.classList.remove('open');
                }
            });
            
            this.classList.toggle('open');
        });
    });

    // Prevent default behavior for submenu links with hash
    const submenuLinks = document.querySelectorAll('.nav-submenu a');
    submenuLinks.forEach(function (link) {
        link.addEventListener('click', function (e) {
            // Only prevent default if href is just a hash
            if (this.getAttribute('href') === '#') {
                e.preventDefault();
                e.stopPropagation();
            }
        });
    });
}

/**
 * Initialize filter functionality
 */
function initializeFilters() {
    // Time Filter
    const timeFilter = document.getElementById('timeFilter');
    if (timeFilter) {
        timeFilter.addEventListener('change', function () {
            loadDashboardData(this.value);
        });
    }

    // Chart Filter Buttons
    const filterButtons = document.querySelectorAll('.filter-btn');
    filterButtons.forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            
            // Remove active class from all buttons
            filterButtons.forEach(function (b) {
                b.classList.remove('active');
            });
            
            // Add active class to clicked button
            this.classList.add('active');
            
            // Update chart
            const filter = this.getAttribute('data-filter');
            updateRevenueChart(filter);
        });
    });
}

/**
 * Initialize interactive elements
 */
function initializeInteractiveElements() {
    // Ensure navbar icons are clickable
    const navbarIcons = document.querySelectorAll('.navbar-icon');
    navbarIcons.forEach(function (icon) {
        icon.style.cursor = 'pointer';
    });

    // User profile dropdown toggle
    const userProfile = document.querySelector('.user-profile');
    if (userProfile) {
        userProfile.style.cursor = 'pointer';
    }

    // Button active states
    const buttons = document.querySelectorAll('button, .btn-action, .btn-view-all');
    buttons.forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
        });
    });
}

/**
 * Load dashboard data based on filter
 */
function loadDashboardData(filter) {
    if (typeof $ !== 'undefined') {
        $.get('/ChuTro/RefreshData', { filter: filter }, function (data) {
            console.log('Dashboard data loaded:', data);
        }).fail(function (error) {
            console.error('Error loading dashboard data:', error);
        });
    } else {
        fetch('/ChuTro/RefreshData?filter=' + encodeURIComponent(filter))
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(data => {
                console.log('Dashboard data loaded:', data);
            })
            .catch(error => console.error('Error loading dashboard data:', error));
    }
}

/**
 * Update revenue chart
 */
function updateRevenueChart(filter) {
    if (typeof $ !== 'undefined') {
        $.get('/ChuTro/GetRevenueChart', { filter: filter }, function (data) {
            if (window.revenueChart && data && data.labels) {
                window.revenueChart.data.labels = data.labels;
                window.revenueChart.data.datasets[0].data = data.values;
                window.revenueChart.update();
            }
        }).fail(function (error) {
            console.error('Error updating revenue chart:', error);
        });
    } else {
        fetch('/ChuTro/GetRevenueChart?filter=' + encodeURIComponent(filter))
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(data => {
                if (window.revenueChart && data && data.labels) {
                    window.revenueChart.data.labels = data.labels;
                    window.revenueChart.data.datasets[0].data = data.values;
                    window.revenueChart.update();
                }
            })
            .catch(error => console.error('Error updating revenue chart:', error));
    }
}

// Handle window resize for responsive behavior
window.addEventListener('resize', debounce(function () {
    const sidebar = document.getElementById('sidebar');
    if (window.innerWidth > 768 && sidebar) {
        sidebar.classList.remove('open');
    }
}, 250));

/**
 * Debounce function to prevent excessive function calls
 */
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}