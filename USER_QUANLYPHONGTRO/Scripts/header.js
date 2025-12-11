/**
 * Header JavaScript
 * Quản lý các tương tác header
 */

(function() {
    'use strict';

    // Active menu item based on current URL
    function setActiveMenuItem() {
        const currentPath = window.location.pathname.toLowerCase();
        const menuItems = document.querySelectorAll('.menu-item');
        
        menuItems.forEach(item => {
            const href = item.getAttribute('href')?.toLowerCase();
            
            // Exact match
            if (href === currentPath) {
                item.classList.add('active');
            }
            // Partial match (for nested pages)
            else if (href && currentPath.includes(href) && href !== '/') {
                item.classList.add('active');
            }
        });
    }

    // Search functionality
    function initializeSearch() {
        const searchForm = document.getElementById('searchForm');
        const searchInput = searchForm?.querySelector('.search-bar');
        
        if (!searchForm || !searchInput) return;

        // Enter key search
        searchInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                searchForm.submit();
            }
        });

        // Clear empty searches
        searchForm.addEventListener('submit', function(e) {
            const query = searchInput.value.trim();
            if (!query) {
                e.preventDefault();
                searchInput.focus();
            }
        });
    }

    // Dropdown behavior
    function initializeDropdown() {
        const accountSection = document.querySelector('.account-section');
        const dropdownMenu = document.querySelector('.dropdown-menu');
        
        if (!accountSection || !dropdownMenu) return;

        // Close dropdown when clicking outside
        document.addEventListener('click', function(e) {
            if (!accountSection.contains(e.target)) {
                dropdownMenu.style.opacity = '0';
                dropdownMenu.style.visibility = 'hidden';
            }
        });

        // Mobile: click to toggle
        const accountTrigger = document.querySelector('.account-trigger');
        if (accountTrigger && window.innerWidth <= 768) {
            accountTrigger.addEventListener('click', function(e) {
                e.stopPropagation();
                const isVisible = dropdownMenu.style.visibility === 'visible';
                dropdownMenu.style.opacity = isVisible ? '0' : '1';
                dropdownMenu.style.visibility = isVisible ? 'hidden' : 'visible';
            });
        }
    }

    // Sticky header behavior
    function initializeStickyHeader() {
        const header = document.querySelector('.app-header');
        const navMenu = document.querySelector('.nav-menu');
        
        if (!header || !navMenu) return;

        let lastScroll = 0;
        
        window.addEventListener('scroll', function() {
            const currentScroll = window.pageYOffset;
            
            // Hide/show on scroll
            if (currentScroll > lastScroll && currentScroll > 100) {
                // Scrolling down
                header.style.transform = 'translateY(-100%)';
            } else {
                // Scrolling up
                header.style.transform = 'translateY(0)';
            }
            
            lastScroll = currentScroll;
        });
    }

    // Badge notification update (for real-time updates)
    function updateNotificationBadge(count) {
        const badges = document.querySelectorAll('.badge');
        badges.forEach(badge => {
            if (count > 0) {
                badge.textContent = count > 99 ? '99+' : count;
                badge.style.display = 'inline-block';
            } else {
                badge.style.display = 'none';
            }
        });
    }

    // Initialize all
    function init() {
        setActiveMenuItem();
        initializeSearch();
        initializeDropdown();
        initializeStickyHeader();
    }

    // Wait for DOM to be ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Expose utility functions globally (optional)
    window.HeaderUtils = {
        updateNotificationBadge: updateNotificationBadge
    };

})();