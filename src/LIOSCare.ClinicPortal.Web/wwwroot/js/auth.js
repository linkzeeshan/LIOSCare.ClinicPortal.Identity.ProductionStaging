// Auth page utilities
(function () {
    // Auto-hide empty validation summary container
    const summary = document.querySelector('.validation-summary-errors');
    if (summary && !summary.innerText.trim()) {
        summary.style.display = 'none';
    }

    // Add focus styles for inputs
    document.querySelectorAll('input').forEach(input => {
        input.addEventListener('invalid', function () {
            this.classList.add('invalid');
        });
        input.addEventListener('input', function () {
            this.classList.remove('invalid');
        });
    });
})();
