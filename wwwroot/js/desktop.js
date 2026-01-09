// Desktop JavaScript

// Set active navigation and handle sidebar toggle
document.addEventListener('DOMContentLoaded', function() {
    // Check if we're on a desktop layout page (has sidebar)
    const sidebar = document.getElementById('sidebar');
    if (!sidebar) {
        // Not a desktop layout page, exit early
        return;
    }
    
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.sidebar-nav .nav-link');
    
    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
            
            // If this is a submenu item, open its parent submenu
            if (link.classList.contains('submenu-item')) {
                const parentGroup = link.closest('.nav-item-group');
                if (parentGroup) {
                    const submenu = parentGroup.querySelector('.submenu');
                    const parentLink = parentGroup.querySelector('.has-submenu');
                    if (submenu && parentLink) {
                        submenu.classList.add('show');
                        parentLink.classList.add('active');
                    }
                }
            }
        }
    });
    
    // Dashboard Submenu toggle
    const dashboardMenu = document.getElementById('dashboardMenu');
    const dashboardSubmenu = document.getElementById('dashboardSubmenu');
    
    if (dashboardMenu && dashboardSubmenu) {
        dashboardMenu.addEventListener('click', function(e) {
            e.preventDefault();
            dashboardSubmenu.classList.toggle('show');
            dashboardMenu.classList.toggle('active');
        });
    }
    
    // Submenu toggle
    const measurementMenu = document.getElementById('measurementMenu');
    const measurementSubmenu = document.getElementById('measurementSubmenu');
    
    if (measurementMenu && measurementSubmenu) {
        measurementMenu.addEventListener('click', function(e) {
            e.preventDefault();
            measurementSubmenu.classList.toggle('show');
            measurementMenu.classList.toggle('active');
        });
    }
    
    // Settings Submenu toggle
    const settingsMenu = document.getElementById('settingsMenu');
    const settingsSubmenu = document.getElementById('settingsSubmenu');
    
    if (settingsMenu && settingsSubmenu) {
        settingsMenu.addEventListener('click', function(e) {
            e.preventDefault();
            settingsSubmenu.classList.toggle('show');
            settingsMenu.classList.toggle('active');
        });
    }
    
    // E_LWP Submenu toggle
    const elwpMenu = document.getElementById('elwpMenu');
    const elwpSubmenu = document.getElementById('elwpSubmenu');
    
    if (elwpMenu && elwpSubmenu) {
        elwpMenu.addEventListener('click', function(e) {
            e.preventDefault();
            elwpSubmenu.classList.toggle('show');
            elwpMenu.classList.toggle('active');
        });
    }
    
    // Sidebar toggle functionality
    const toggleBtn = document.getElementById('toggleSidebar');
    const mainContent = document.getElementById('mainContent');
    
    // Check if sidebar state is saved in localStorage
    if (sidebar && mainContent) {
    const sidebarCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
    if (sidebarCollapsed) {
        sidebar.classList.add('collapsed');
        mainContent.classList.add('sidebar-collapsed');
        }
    }
    
    if (toggleBtn && sidebar && mainContent) {
        toggleBtn.addEventListener('click', function() {
            sidebar.classList.toggle('collapsed');
            mainContent.classList.toggle('sidebar-collapsed');
            
            // Save state to localStorage
            const isCollapsed = sidebar.classList.contains('collapsed');
            localStorage.setItem('sidebarCollapsed', isCollapsed);
        });
    }
    
    // Mobile/Tablet sidebar toggle
    const btnSidebarMobile = document.getElementById('btnSidebarMobile');
    const sidebarOverlay = document.getElementById('sidebarOverlay');
    
    if (btnSidebarMobile && sidebarOverlay && sidebar) {
        // Toggle sidebar
        btnSidebarMobile.addEventListener('click', function(e) {
            e.stopPropagation();
            sidebar.classList.toggle('show');
            sidebarOverlay.classList.toggle('show');
        });
        
        // Close sidebar when overlay is clicked (but not when clicking inside sidebar)
        sidebarOverlay.addEventListener('click', function(e) {
            // Only close if clicking directly on overlay, not on sidebar content
            if (e.target === sidebarOverlay) {
                sidebar.classList.remove('show');
                sidebarOverlay.classList.remove('show');
            }
        });
        
        // Prevent sidebar from closing when clicking inside sidebar
        sidebar.addEventListener('click', function(e) {
            e.stopPropagation();
        });
        
        // Prevent submenu clicks from closing sidebar
        const submenuLinks = sidebar.querySelectorAll('.submenu .nav-link');
        submenuLinks.forEach(link => {
            link.addEventListener('click', function(e) {
                e.stopPropagation();
            });
        });
        
        // Close sidebar when clicking a nav link on mobile/tablet (only for actual navigation links, not submenu toggles)
        // Use event delegation to handle dynamically added links
        const handleNavLinkClick = function(e) {
            const link = e.currentTarget;
            // Only close if it's not a submenu toggle and has a valid href that navigates
            if (!link.classList.contains('has-submenu') && 
                link.getAttribute('href') && 
                link.getAttribute('href') !== '#' &&
                !link.closest('.submenu')) {
                // Small delay to allow navigation to happen
                setTimeout(() => {
                    if (window.innerWidth <= 1024 && sidebar && sidebarOverlay) {
                        sidebar.classList.remove('show');
                        sidebarOverlay.classList.remove('show');
                    }
                }, 100);
            }
        };
        
        // Add event listeners to all nav links
        navLinks.forEach(link => {
            // Remove any existing listeners first
            link.removeEventListener('click', handleNavLinkClick);
            link.addEventListener('click', handleNavLinkClick);
        });
    }
});

// Confirm delete actions
function confirmDelete(message) {
    return confirm(message || 'Apakah Anda yakin ingin menghapus data ini?');
}

// Toast notification (simple)
function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `toast-notification toast-${type}`;
    toast.textContent = message;
    toast.style.cssText = `
        position: fixed;
        top: 90px;
        right: 30px;
        padding: 16px 24px;
        background-color: ${type === 'success' ? '#10b981' : '#ef4444'};
        color: white;
        border-radius: 8px;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        z-index: 10000;
        animation: slideIn 0.3s ease-out;
    `;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
        toast.style.animation = 'slideOut 0.3s ease-out';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// Add CSS animations
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);
