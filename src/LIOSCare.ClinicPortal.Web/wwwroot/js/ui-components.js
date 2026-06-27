/* Shared UI utilities: toast notifications, mobile helpers, loading states */
(function () {
    const container = document.createElement('div');
    container.id = 'toast-container';
    document.body.appendChild(container);

    function createToast(type, title, message, duration = 5000) {
        const icons = {
            success: 'bi-check-circle-fill',
            error: 'bi-x-circle-fill',
            warning: 'bi-exclamation-triangle-fill',
            info: 'bi-info-circle-fill'
        };

        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        toast.innerHTML = `
            <div class="toast-icon"><i class="bi ${icons[type] || icons.info}"></i></div>
            <div class="toast-content">
                <strong>${escapeHtml(title)}</strong>
                <span>${escapeHtml(message)}</span>
            </div>
            <button class="toast-close" aria-label="Close">&times;</button>
        `;

        toast.querySelector('.toast-close').addEventListener('click', () => removeToast(toast));
        container.appendChild(toast);

        if (duration > 0) {
            setTimeout(() => removeToast(toast), duration);
        }
    }

    function removeToast(toast) {
        toast.classList.add('hiding');
        toast.addEventListener('animationend', () => toast.remove());
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Global toast API
    window.showToast = createToast;
    window.showSuccessToast = (title, message, duration) => createToast('success', title, message, duration);
    window.showErrorToast = (title, message, duration) => createToast('error', title, message, duration);
    window.showWarningToast = (title, message, duration) => createToast('warning', title, message, duration);
    window.showInfoToast = (title, message, duration) => createToast('info', title, message, duration);

    // Convert TempData success messages into toasts
    document.querySelectorAll('[data-toast-title]').forEach(el => {
        const type = el.dataset.toastType || 'info';
        const title = el.dataset.toastTitle || '';
        const message = el.dataset.toastMessage || '';
        createToast(type, title, message);
        el.remove();
    });

    // Auto-show server-side alerts as toasts if marked with .alert-toast
    document.querySelectorAll('.alert-toast').forEach(el => {
        const type = el.classList.contains('alert-success') ? 'success'
            : el.classList.contains('alert-danger') ? 'error'
            : el.classList.contains('alert-warning') ? 'warning'
            : 'info';
        const title = el.querySelector('strong')?.textContent || type.charAt(0).toUpperCase() + type.slice(1);
        const message = Array.from(el.childNodes)
            .filter(n => n.nodeType === Node.TEXT_NODE || (n.nodeType === Node.ELEMENT_NODE && n.tagName !== 'STRONG'))
            .map(n => n.textContent)
            .join(' ')
            .trim();
        createToast(type, title, message);
        el.style.display = 'none';
    });
})();
