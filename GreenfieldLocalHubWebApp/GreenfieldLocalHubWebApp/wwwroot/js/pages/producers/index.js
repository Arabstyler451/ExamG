(function () {
    const searchInput = document.getElementById('producerSearch');
    const grid = document.getElementById('producerGrid');
    const countEl = document.getElementById('visibleCount');
    if (!searchInput || !grid) return;

    searchInput.addEventListener('input', function () {
        const q = this.value.trim().toLowerCase();
        const cards = grid.querySelectorAll('.producer-card');
        let visible = 0;

        cards.forEach(function (card) {
            const show = q === '' || card.textContent.toLowerCase().includes(q);
            card.style.display = show ? '' : 'none';
            if (show) visible++;
        });

        if (countEl) countEl.textContent = visible;
    });
})();