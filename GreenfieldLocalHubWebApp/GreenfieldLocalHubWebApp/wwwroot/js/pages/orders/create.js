(function () {
    /* ── Config ─────────────────────────────────────────────── */
    const CFG = {
        deliveryFees: { Standard: 3.50, "First Class": 5.50, "Next Day": 7.99 },
        defaultDeliveryFee: 3.50,
        freeDeliveryThreshold: 30.00,
        minCollectionDaysAhead: 2,
    };

    /* ── Server data ─────────────────────────────────────────── */
    const sd = window.checkoutData ?? {};
    const subtotalBefore = parseFloat(sd.subtotalBefore) || 0;
    const loyaltyDiscount = parseFloat(sd.loyaltyDiscount) || 0;
    const discountedSubtotal = parseFloat(sd.discountedSubtotal) || 0;

    /* ── DOM refs ────────────────────────────────────────────── */
    const $ = id => document.getElementById(id);
    const deliveryCheck = $('deliveryCheck');
    const collectionCheck = $('collectionCheck');
    const tileDelivery = $('tile-delivery');
    const tileCollection = $('tile-collection');
    const deliveryTypeGrp = $('deliveryTypeGroup');
    const collectionDateGrp = $('orderCollectionDateGroup');
    const collectionDateEl = $('collectionDate');
    const addressCard = $('addressCard');
    const summaryText = $('summaryFulfilmentText');
    const deliveryTypeSelect = $('deliveryTypeSelect');
    const fulfilmentInput = $('FulfilmentChoice');
    const subtotalBeforeEl = $('subtotal-before');
    const loyaltyDiscountEl = $('loyalty-discount');
    const discountedSubEl = $('discounted-subtotal');
    const deliveryFeeEl = $('delivery-fee');
    const grandTotalEl = $('grand-total');
    const checkoutForm = $('checkoutForm');

    /* ── Helpers ─────────────────────────────────────────────── */
    function toggle(el, condition, prop = 'visible') {
        if (!el) return;
        if (prop === 'visible') el.classList.toggle('visible', condition);
        else if (prop === 'display') el.style.display = condition ? '' : 'none';
        else if (prop === 'disabled') el.disabled = !condition;
    }

    /* ── Address Validation ─────────────────────────────────── */
    function validateAddressForDelivery() {
        const isDelivery = fulfilmentInput?.value === 'Delivery';
        const addressSelect = document.getElementById('addressId');
        const addressError = document.querySelector('[data-valmsg-for="addressId"]');

        if (isDelivery && addressSelect) {
            const hasAddress = addressSelect.value && addressSelect.value !== '';

            if (!hasAddress) {
                if (addressError) {
                    addressError.textContent = "Please select a delivery address";
                    addressError.style.display = "block";
                }
                addressSelect.classList.add('input-validation-error');
                return false;
            } else {
                if (addressError) {
                    addressError.textContent = "";
                    addressError.style.display = "none";
                }
                addressSelect.classList.remove('input-validation-error');
                return true;
            }
        }
        return true;
    }

    /* ── Collection Date Validation ─────────────────────────── */
    function validateCollectionDate() {
        const isCollection = fulfilmentInput?.value === 'Collection';
        const collectionDate = document.getElementById('collectionDate');
        const collectionDateError = document.querySelector('[data-valmsg-for="orderCollectionDate"]');

        if (isCollection && collectionDate) {
            const hasDate = collectionDate.value && collectionDate.value !== '';

            if (!hasDate) {
                if (collectionDateError) {
                    collectionDateError.textContent = "Please select a collection date";
                    collectionDateError.style.display = "block";
                }
                collectionDate.classList.add('input-validation-error');
                return false;
            } else {
                // Check if date is at least 2 days from now
                const selectedDate = new Date(collectionDate.value);
                const today = new Date();
                today.setHours(0, 0, 0, 0);

                const minDate = new Date();
                minDate.setDate(minDate.getDate() + CFG.minCollectionDaysAhead);
                minDate.setHours(0, 0, 0, 0);

                if (selectedDate < minDate) {
                    if (collectionDateError) {
                        collectionDateError.textContent = `Collection date must be at least ${CFG.minCollectionDaysAhead} days from today`;
                        collectionDateError.style.display = "block";
                    }
                    collectionDate.classList.add('input-validation-error');
                    return false;
                }

                // Clear error
                if (collectionDateError) {
                    collectionDateError.textContent = "";
                    collectionDateError.style.display = "none";
                }
                collectionDate.classList.remove('input-validation-error');
                return true;
            }
        }
        return true;
    }

    /* ── Initialise collection date min ─────────────────────── */
    if (collectionDateEl) {
        const min = new Date();
        min.setDate(min.getDate() + CFG.minCollectionDaysAhead);
        collectionDateEl.min = min.toISOString().split('T')[0];
    }

    /* ── Sub-renderers ───────────────────────────────────────── */
    function updateTiles(isDelivery, isCollection) {
        tileDelivery?.classList.toggle('selected', isDelivery);
        tileCollection?.classList.toggle('selected', isCollection);
    }

    function updatePanels(isDelivery, isCollection) {
        toggle(deliveryTypeGrp, isDelivery, 'visible');
        toggle(collectionDateGrp, isCollection, 'visible');
        toggle(deliveryTypeSelect, isDelivery, 'disabled');
        toggle(collectionDateEl, isCollection, 'disabled');
        toggle(addressCard, !isCollection, 'display');
    }

    function updateSummaryText(isDelivery, isCollection) {
        if (!summaryText) return;
        if (isDelivery) {
            const label = deliveryTypeSelect?.value
                ? deliveryTypeSelect.options[deliveryTypeSelect.selectedIndex].text
                : 'Home Delivery';
            summaryText.textContent = `Home Delivery — ${label}`;
        } else if (isCollection) {
            let datePart = 'date TBC';
            if (collectionDateEl?.valueAsDate) {
                const d = new Date(collectionDateEl.valueAsDate.getTime()
                    + collectionDateEl.valueAsDate.getTimezoneOffset() * 60000);
                datePart = d.toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' });
            }
            summaryText.textContent = `Click & Collect — ${datePart}`;
        } else {
            summaryText.textContent = 'No method selected';
        }
    }

    const hasFreeDelivery = window.checkoutData?.hasFreeDelivery === true;

    function updateTotals(isDelivery) {
        let deliveryFee = 0;
        if (isDelivery && deliveryTypeSelect?.value) {
            deliveryFee = hasFreeDelivery || discountedSubtotal >= CFG.freeDeliveryThreshold
                ? 0
                : (CFG.deliveryFees[deliveryTypeSelect.value] ?? CFG.defaultDeliveryFee);
        }

        const grandTotal = discountedSubtotal + deliveryFee;

        if (subtotalBeforeEl) subtotalBeforeEl.textContent = `£${subtotalBefore.toFixed(2)}`;
        if (discountedSubEl) discountedSubEl.textContent = `£${discountedSubtotal.toFixed(2)}`;
        if (loyaltyDiscountEl) loyaltyDiscountEl.textContent = `−£${loyaltyDiscount.toFixed(2)}`;
        if (deliveryFeeEl) {
            deliveryFeeEl.innerHTML = deliveryFee === 0
                ? '<span style="color:#1a6b40;">Free</span>'
                : `£${deliveryFee.toFixed(2)}`;
        }
        if (grandTotalEl) grandTotalEl.textContent = `£${grandTotal.toFixed(2)}`;
    }

    /* ── Main update ─────────────────────────────────────────── */
    function update() {
        const isDelivery = fulfilmentInput?.value === 'Delivery';
        const isCollection = fulfilmentInput?.value === 'Collection';
        updateTiles(isDelivery, isCollection);
        updatePanels(isDelivery, isCollection);
        updateSummaryText(isDelivery, isCollection);
        updateTotals(isDelivery);
        // Re-validate when fulfilment changes
        validateAddressForDelivery();
        validateCollectionDate();
    }

    /* ── Tile click handlers ─────────────────────────────────── */
    tileDelivery?.addEventListener('click', e => {
        e.preventDefault();
        if (fulfilmentInput) fulfilmentInput.value = 'Delivery';
        update();
    });

    tileCollection?.addEventListener('click', e => {
        e.preventDefault();
        if (fulfilmentInput) fulfilmentInput.value = 'Collection';
        update();
    });

    /* ── Change listeners ────────────────────────────────────── */
    deliveryTypeSelect?.addEventListener('change', update);
    collectionDateEl?.addEventListener('change', function () {
        update();
        validateCollectionDate();
    });

    /* ── Form submission validation ─────────────────────────── */
    if (checkoutForm) {
        checkoutForm.addEventListener('submit', function (e) {
            const isAddressValid = validateAddressForDelivery();
            const isCollectionDateValid = validateCollectionDate();

            if (!isAddressValid || !isCollectionDateValid) {
                e.preventDefault();

                // Scroll to the appropriate section
                if (!isCollectionDateValid && fulfilmentInput?.value === 'Collection') {
                    const collectionSection = document.getElementById('orderCollectionDateGroup');
                    if (collectionSection) {
                        collectionSection.scrollIntoView({ behavior: 'smooth', block: 'center' });
                        const collectionDateField = document.getElementById('collectionDate');
                        if (collectionDateField) {
                            collectionDateField.focus();
                        }
                    }
                } else if (!isAddressValid && fulfilmentInput?.value === 'Delivery') {
                    document.getElementById('addressCard')?.scrollIntoView({
                        behavior: 'smooth',
                        block: 'center'
                    });
                }
            }
        });
    }

    /* ── Real-time validation events ────────────────────────── */
    const addressSelect = document.getElementById('addressId');
    if (addressSelect) {
        addressSelect.addEventListener('change', validateAddressForDelivery);
        addressSelect.addEventListener('blur', validateAddressForDelivery);
    }

    const collectionDateField = document.getElementById('collectionDate');
    if (collectionDateField) {
        collectionDateField.addEventListener('change', validateCollectionDate);
        collectionDateField.addEventListener('blur', validateCollectionDate);
    }

    /* ── Sync FulfilmentChoice on load ───────────────────────── */
    if (fulfilmentInput) {
        if (document.getElementById('deliveryCheck')?.value === 'true')
            fulfilmentInput.value = 'Delivery';
        if (document.getElementById('collectionCheck')?.value === 'true')
            fulfilmentInput.value = 'Collection';
    }

    /* ── Initial render ──────────────────────────────────────── */
    update();

    console.log('✅ Checkout JS loaded with address and collection date validation');
    console.log('shoppingCartId:', document.querySelector('input[name="shoppingCartId"]')?.value);
})();