/* ACCESSIBILITY STATE */
let fontSize = parseInt(localStorage.getItem('fontSize')) || 16;
let isSpeaking = false;
let utterance;

/* INIT (LOAD SAVED SETTINGS) */
(function () {
    if (localStorage.getItem('darkMode') === 'true') {
        document.body.classList.add('dark-mode');
    }

    if (localStorage.getItem('highContrast') === 'true') {
        document.body.classList.add('high-contrast');
    }

    applyFontSize();
})();

/* DARK MODE */
function toggleDarkMode() {
    const active = document.body.classList.toggle('dark-mode');
    localStorage.setItem('darkMode', active);
    showToast(active ? 'Dark mode enabled.' : 'Dark mode disabled.');
}

/* HIGH CONTRAST */
function toggleHighContrast() {
    const active = document.body.classList.toggle('high-contrast');
    localStorage.setItem('highContrast', active);
    showToast(active ? 'High contrast enabled.' : 'High contrast disabled.');
}

/* TEXT SIZE */
function applyFontSize() {
    document.documentElement.style.fontSize = fontSize + 'px';
    localStorage.setItem('fontSize', fontSize);
}

function changeTextSize(delta) {
    fontSize = Math.min(22, Math.max(14, fontSize + delta));
    applyFontSize();
    showToast('Text size: ' + fontSize + 'px');
}

/* TEXT TO SPEECH */
function toggleSpeech() {
    const btn = document.getElementById('ttsBtn');

    if (isSpeaking) {
        speechSynthesis.cancel();
        isSpeaking = false;
        btn.setAttribute('aria-pressed', 'false');
        showToast('Speech stopped.');
        return;
    }

    const text = getReadableText();

    if (!text) {
        showToast('No readable content found.');
        return;
    }

    utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = 'en-GB';
    utterance.rate = 1;
    utterance.pitch = 1;

    utterance.onend = function () {
        isSpeaking = false;
        btn.setAttribute('aria-pressed', 'false');
    };

    speechSynthesis.speak(utterance);

    isSpeaking = true;
    btn.setAttribute('aria-pressed', 'true');
    showToast('Reading page...');
}

function getReadableText() {
    const main = document.getElementById('main-content');

    if (!main) return document.body.innerText;

    return main.innerText.replace(/\s+/g, ' ').trim();
}

/* RESET SETTINGS */
function resetAccessibility() {
    localStorage.clear();
    speechSynthesis.cancel();
    location.reload();
}

/* MOBILE NAV */
function toggleMobileNav(btn) {
    const nav = document.getElementById('mobile-nav');
    const open = nav.style.display === 'block';

    nav.style.display = open ? 'none' : 'block';
    btn.setAttribute('aria-expanded', !open);

    if (!open) {
        nav.querySelector('a')?.focus();
    }
}

/* TOAST */
function showToast(msg) {
    const t = document.getElementById('toast');
    t.textContent = msg;
    t.classList.add('show');
    setTimeout(() => t.classList.remove('show'), 3500);
}

/* ACTIVE NAV LINK */
(function () {
    const currentPath = window.location.pathname.toLowerCase();

    const normalizedPath = currentPath.endsWith('/') && currentPath !== '/'
        ? currentPath.slice(0, -1)
        : currentPath;

    document.querySelectorAll('.nav-links .nav-link').forEach(function (link) {
        const href = link.getAttribute('href');

        if (href && href !== '#') {
            let linkPath = href.toLowerCase().split('?')[0].split('#')[0];

            if (linkPath.startsWith('./')) linkPath = linkPath.slice(1);
            if (linkPath.startsWith('../')) linkPath = linkPath.slice(2);

            let normalizedLink = linkPath.endsWith('/') && linkPath !== '/'
                ? linkPath.slice(0, -1)
                : linkPath;

            if (normalizedLink === '') normalizedLink = '/';

            if (normalizedPath === normalizedLink ||
                (normalizedPath === '' && normalizedLink === '/') ||
                (normalizedLink !== '/' && normalizedPath === normalizedLink)) {
                link.classList.add('active');
            } else {
                link.classList.remove('active');
            }
        }
    });
})();