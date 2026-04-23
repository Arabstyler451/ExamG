document.querySelectorAll('.product-card').forEach(card => {
    card.style.cursor = 'pointer';
    card.addEventListener('click', (e) => {
        // Don't trigger if clicking on buttons, forms, or links
        if (e.target.closest('.btn') ||
            e.target.closest('form') ||
            e.target.closest('button') ||
            e.target.closest('a')) {
            return;
        }

        const productId = card.dataset.productId;
        window.location.href = `/products/Details/${productId}`;
    });
});