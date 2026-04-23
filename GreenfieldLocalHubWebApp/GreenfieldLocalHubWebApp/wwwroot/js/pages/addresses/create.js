(function () {
    'use strict';

    /* ── Postcode real-time validation ── */
    const postcodeInput = document.getElementById('postalCode');
    const statusBox = document.getElementById('postcodeStatus');
    const statusText = document.getElementById('postcodeStatusText');

    function setPostcodeStatus(state, message) {
        statusBox.className = 'addr-postcode-status ' + state;
        statusText.textContent = message;
    }

    if (postcodeInput) {

        postcodeInput.addEventListener('blur', async function () {
            const raw = this.value.trim();
            const stripped = raw.replace(/\s+/g, '').toUpperCase();

            if (!stripped) return;

            /* Basic UK postcode format check before hitting the API */
            const ukPattern = /^[A-Z]{1,2}\d[A-Z\d]?\s?\d[A-Z]{2}$/i;
            if (!ukPattern.test(stripped)) {
                setPostcodeStatus('invalid', 'Format does not look like a UK post code');
                this.classList.remove('input-valid');
                this.classList.add('input-invalid');
                this.setCustomValidity('Please enter a valid UK post code (e.g. B1 1BB).');
                return;
            }

            setPostcodeStatus('checking', 'Checking post code…');

            try {
                const res = await fetch(`https://api.postcodes.io/postcodes/${encodeURIComponent(stripped)}/validate`);
                const data = await res.json();

                if (data.result) {
                    setPostcodeStatus('valid', 'Valid UK post code');
                    this.classList.remove('input-invalid');
                    this.classList.add('input-valid');
                    this.setCustomValidity('');

                    /* Auto-format: insert space if missing (e.g. B11BB → B1 1BB) */
                    if (!raw.includes(' ') && stripped.length >= 5) {
                        const inward = stripped.slice(-3);
                        const outward = stripped.slice(0, stripped.length - 3);
                        this.value = outward + ' ' + inward;
                    }
                } else {
                    setPostcodeStatus('invalid', 'Post code not found — please check it');
                    this.classList.remove('input-valid');
                    this.classList.add('input-invalid');
                    this.setCustomValidity('Post code not found in Royal Mail database.');
                }
            } catch {
                /* If the API is unreachable, fall back silently — don't block submit */
                setPostcodeStatus('', '');
                this.classList.remove('input-valid', 'input-invalid');
                this.setCustomValidity('');
            }
        });

        /* Reset on new input */
        postcodeInput.addEventListener('input', function () {
            statusBox.className = 'addr-postcode-status';
            this.classList.remove('input-valid', 'input-invalid');
            this.setCustomValidity('');
        });
    }

    /* ── Street: letters, numbers, spaces, commas, hyphens only ── */
    const streetInput = document.querySelector('input[name="street"]');
    if (streetInput) {
        streetInput.addEventListener('blur', function () {
            const val = this.value.trim();
            if (val && !/^[A-Za-z0-9 ,.\-']+$/.test(val)) {
                this.setCustomValidity('Street address contains invalid characters.');
                this.classList.add('input-invalid');
            } else {
                this.setCustomValidity('');
                this.classList.remove('input-invalid');
                if (val) this.classList.add('input-valid');
            }
        });
        streetInput.addEventListener('input', function () {
            this.setCustomValidity('');
            this.classList.remove('input-valid', 'input-invalid');
        });
    }

    /* ── City: letters, spaces, hyphens only ── */
    const cityInput = document.querySelector('input[name="city"]');
    if (cityInput) {
        cityInput.addEventListener('blur', function () {
            const val = this.value.trim();
            if (val && !/^[A-Za-z \-']+$/.test(val)) {
                this.setCustomValidity('City name should only contain letters.');
                this.classList.add('input-invalid');
            } else {
                this.setCustomValidity('');
                this.classList.remove('input-invalid');
                if (val) this.classList.add('input-valid');
            }
        });
        cityInput.addEventListener('input', function () {
            this.setCustomValidity('');
            this.classList.remove('input-valid', 'input-invalid');
        });
    }

})();