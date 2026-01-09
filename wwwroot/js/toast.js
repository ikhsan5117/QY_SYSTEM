// Modern Toast Notification System
class Toast {
    constructor() {
        this.createContainer();
    }

    createContainer() {
        if (!document.getElementById('toast-container')) {
            const container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'toast-container';
            document.body.appendChild(container);
        }
    }

    show(message, type = 'success', duration = 3000) {
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        
        const icons = {
            success: '<i class="bi bi-check-circle-fill"></i>',
            error: '<i class="bi bi-x-circle-fill"></i>',
            warning: '<i class="bi bi-exclamation-triangle-fill"></i>',
            info: '<i class="bi bi-info-circle-fill"></i>'
        };

        toast.innerHTML = `
            <div class="toast-icon">${icons[type] || icons.info}</div>
            <div class="toast-content">
                <div class="toast-message">${message}</div>
            </div>
            <button class="toast-close" onclick="this.parentElement.remove()">
                <i class="bi bi-x"></i>
            </button>
        `;

        const container = document.getElementById('toast-container');
        container.appendChild(toast);

        // Trigger animation
        setTimeout(() => toast.classList.add('show'), 10);

        // Auto remove
        if (duration > 0) {
            setTimeout(() => {
                toast.classList.remove('show');
                setTimeout(() => toast.remove(), 300);
            }, duration);
        }

        return toast;
    }

    success(message, duration = 3000) {
        return this.show(message, 'success', duration);
    }

    error(message, duration = 4000) {
        return this.show(message, 'error', duration);
    }

    warning(message, duration = 3500) {
        return this.show(message, 'warning', duration);
    }

    info(message, duration = 3000) {
        return this.show(message, 'info', duration);
    }

    confirm(message, onConfirm, onCancel) {
        const modal = document.createElement('div');
        modal.className = 'toast-modal-overlay';
        
        modal.innerHTML = `
            <div class="toast-modal">
                <div class="toast-modal-icon toast-modal-icon-warning">
                    <i class="bi bi-exclamation-triangle-fill"></i>
                </div>
                <div class="toast-modal-title">Konfirmasi</div>
                <div class="toast-modal-message">${message}</div>
                <div class="toast-modal-actions">
                    <button class="toast-modal-btn toast-modal-btn-cancel" onclick="this.closest('.toast-modal-overlay').remove(); ${onCancel ? 'onCancel()' : ''}">
                        <i class="bi bi-x-circle"></i> Batal
                    </button>
                    <button class="toast-modal-btn toast-modal-btn-confirm" onclick="this.closest('.toast-modal-overlay').remove(); onConfirm()">
                        <i class="bi bi-check-circle"></i> Ya, Lanjutkan
                    </button>
                </div>
            </div>
        `;

        document.body.appendChild(modal);
        
        // Store callbacks
        window.onConfirm = onConfirm;
        if (onCancel) window.onCancel = onCancel;

        setTimeout(() => modal.classList.add('show'), 10);

        return modal;
    }

    alert(message, type = 'info', title = '') {
        const modal = document.createElement('div');
        modal.className = 'toast-modal-overlay';
        
        const icons = {
            success: '<i class="bi bi-check-circle-fill"></i>',
            error: '<i class="bi bi-x-circle-fill"></i>',
            warning: '<i class="bi bi-exclamation-triangle-fill"></i>',
            info: '<i class="bi bi-info-circle-fill"></i>'
        };

        const titles = {
            success: 'Berhasil',
            error: 'Error',
            warning: 'Peringatan',
            info: 'Informasi'
        };
        
        modal.innerHTML = `
            <div class="toast-modal">
                <div class="toast-modal-icon toast-modal-icon-${type}">
                    ${icons[type] || icons.info}
                </div>
                <div class="toast-modal-title">${title || titles[type]}</div>
                <div class="toast-modal-message">${message}</div>
                <div class="toast-modal-actions">
                    <button class="toast-modal-btn toast-modal-btn-primary" onclick="this.closest('.toast-modal-overlay').remove()">
                        <i class="bi bi-check-circle"></i> OK
                    </button>
                </div>
            </div>
        `;

        document.body.appendChild(modal);
        setTimeout(() => modal.classList.add('show'), 10);

        return modal;
    }
}

// Create global instance
window.toast = new Toast();

// Override default alert and confirm
window.showToast = (message, type = 'success') => toast.show(message, type);
window.toastSuccess = (message) => toast.success(message);
window.toastError = (message) => toast.error(message);
window.toastWarning = (message) => toast.warning(message);
window.toastInfo = (message) => toast.info(message);
