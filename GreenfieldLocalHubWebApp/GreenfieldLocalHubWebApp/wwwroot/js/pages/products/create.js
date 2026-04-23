function previewImage(url) {
    const img = document.getElementById('imgPreview');
    const placeholder = document.getElementById('imgPlaceholder');
    if (url) {
        img.src = url;
        img.style.display = 'block';
        placeholder.style.display = 'none';
    } else {
        img.style.display = 'none';
        placeholder.style.display = 'flex';
    }
}

function updateChecklist() {
    const val = (id) => document.querySelector(`[name="${id}"]`)?.value?.trim();
    const sel = (id) => document.querySelector(`[name="${id}"]`)?.value;

    const checks = {
        'chk-name': !!val('productName'),
        'chk-desc': !!val('productDescription'),
        'chk-price': !!val('productPrice') && parseFloat(val('productPrice')) > 0,
        'chk-stock': !!val('stockQuantity'),
        'chk-cat': !!sel('categoriesId'),
        'chk-img': !!val('productImage'),
    };

    Object.entries(checks).forEach(([id, done]) => {
        document.getElementById(id)?.classList.toggle('done', done);
    });
}

// Run on load in case of validation postback
updateChecklist();