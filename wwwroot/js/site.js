// Main JavaScript file for Check Dimensi App

// Fungsi untuk validasi input real-time
function validateInput(inputElement, min, max, statusElement) {
    inputElement.addEventListener('input', function(e) {
        const nilai = parseFloat(e.target.value);
        
        if (isNaN(nilai)) {
            statusElement.classList.remove('status-ok', 'status-ng');
            return;
        }
        
        if (nilai >= min && nilai <= max) {
            statusElement.classList.remove('status-ng');
            statusElement.classList.add('status-ok');
            statusElement.innerHTML = '<i class="bi bi-check-circle"></i> <span>OK</span>';
        } else {
            statusElement.classList.remove('status-ok');
            statusElement.classList.add('status-ng');
            statusElement.innerHTML = '<i class="bi bi-x-circle"></i> <span>NG</span>';
        }
    });
}

// Fungsi untuk format angka
function formatNumber(num, decimals = 2) {
    return parseFloat(num).toFixed(decimals);
}

// Fungsi untuk highlight active navigation
function setActiveNav() {
    const currentPath = window.location.pathname;
    const navItems = document.querySelectorAll('.bottom-nav .nav-item');
    
    navItems.forEach(item => {
        if (item.getAttribute('href') === currentPath) {
            item.classList.add('active');
        } else {
            item.classList.remove('active');
        }
    });
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    setActiveNav();
    
    // Tambahkan smooth scroll behavior
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});

// Fungsi untuk show loading
function showLoading() {
    // Implementasi loading indicator
    console.log('Loading...');
}

// Fungsi untuk hide loading
function hideLoading() {
    // Implementasi hide loading
    console.log('Loading complete');
}

// Export functions untuk digunakan di file lain
window.CheckDimensiApp = {
    validateInput,
    formatNumber,
    showLoading,
    hideLoading
};
