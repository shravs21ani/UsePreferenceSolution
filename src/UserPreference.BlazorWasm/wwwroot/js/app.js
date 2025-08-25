// Theme management
window.applyTheme = function (theme) {
    const body = document.body;
    
    // Remove existing theme attributes
    body.removeAttribute('data-theme');
    
    if (theme === 'dark') {
        body.setAttribute('data-theme', 'dark');
        localStorage.setItem('theme', 'dark');
    } else if (theme === 'auto') {
        // Check system preference
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        const systemTheme = prefersDark ? 'dark' : 'light';
        body.setAttribute('data-theme', systemTheme);
        localStorage.setItem('theme', 'auto');
    } else {
        // Light theme (default)
        localStorage.setItem('theme', 'light');
    }
};

// Initialize theme on page load
window.initializeTheme = function () {
    const savedTheme = localStorage.getItem('theme') || 'light';
    window.applyTheme(savedTheme);
    
    // Listen for system theme changes
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
        const savedTheme = localStorage.getItem('theme');
        if (savedTheme === 'auto') {
            window.applyTheme('auto');
        }
    });
};

// Toast notifications
window.showToast = function (type, message) {
    // Create toast element
    const toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) return;
    
    const toastId = 'toast-' + Date.now();
    const toastHtml = `
        <div id="${toastId}" class="toast show" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header bg-${type} text-white">
                <strong class="me-auto">${getToastTitle(type)}</strong>
                <button type="button" class="btn-close btn-close-white" onclick="removeToast('${toastId}')"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>
    `;
    
    toastContainer.insertAdjacentHTML('beforeend', toastHtml);
    
    // Auto-remove after 5 seconds
    setTimeout(() => {
        removeToast(toastId);
    }, 5000);
};

window.removeToast = function (toastId) {
    const toast = document.getElementById(toastId);
    if (toast) {
        toast.remove();
    }
};

function getToastTitle(type) {
    switch (type) {
        case 'success': return 'Success';
        case 'error': return 'Error';
        case 'warning': return 'Warning';
        case 'info': return 'Information';
        default: return 'Message';
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    window.initializeTheme();
});

// Blazor specific initialization
window.blazorInitialized = function () {
    window.initializeTheme();
};
