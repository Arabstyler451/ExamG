function previewImage(url) {
    const wrap = document.getElementById('imgPreviewWrap');
    const img = document.getElementById('imgPreview');
    if (url) {
        wrap.style.display = 'block';
        img.style.display = 'block';
        img.src = url;
    } else {
        wrap.style.display = 'none';
    }
}