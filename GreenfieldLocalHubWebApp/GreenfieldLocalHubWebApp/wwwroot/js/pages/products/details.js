function glhAdjustQty(delta) {
    const input = document.getElementById('glhQty');
    if (!input) return;
    const max = parseInt(input.max) || 20;
    input.value = Math.max(1, Math.min(max, parseInt(input.value) + delta));
}