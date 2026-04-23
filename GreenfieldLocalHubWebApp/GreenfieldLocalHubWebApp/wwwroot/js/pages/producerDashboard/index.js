/* Tab switching */
function switchDashTab(btn, tabId) {
    document.querySelectorAll('.dtab').forEach(t => t.classList.remove('active'));
    document.querySelectorAll('.tab').forEach(t => {
        t.classList.remove('active');
        t.setAttribute('aria-selected', 'false');
    });
    const target = document.getElementById(tabId);
    if (target) target.classList.add('active');
    btn.classList.add('active');
    btn.setAttribute('aria-selected', 'true');
}

/* Live search filter */
function filterTable(input, tbodyId) {
    const q = input.value.toLowerCase();
    document.querySelectorAll('#' + tbodyId + ' tr').forEach(row => {
        row.style.display = row.textContent.toLowerCase().includes(q) ? '' : 'none';
    });
}

/* Status dropdown filter */
function filterByStatus(select, tbodyId) {
    const val = select.value.toLowerCase();
    document.querySelectorAll('#' + tbodyId + ' tr').forEach(row => {
        row.style.display = (!val || row.textContent.toLowerCase().includes(val)) ? '' : 'none';
    });
}