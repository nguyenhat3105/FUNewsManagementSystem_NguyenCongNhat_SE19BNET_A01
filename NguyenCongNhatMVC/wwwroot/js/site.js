/* ================================================================
   FUNews — Custom UI Dialogs
   Replaces native browser alert() / confirm() with styled modals
   ================================================================ */

// ── CONFIRM MODAL ────────────────────────────────────────────────
(function () {
  let _pendingForm = null;

  function getModal() { return document.getElementById('fu-confirm-modal'); }

  window.fuConfirm = function (opts) {
    const modal = getModal();
    if (!modal) return;

    const icon    = modal.querySelector('.fu-modal-icon');
    const title   = modal.querySelector('.fu-modal-title');
    const message = modal.querySelector('.fu-modal-message');
    const btnOk   = modal.querySelector('.fu-modal-ok');
    const btnCan  = modal.querySelector('.fu-modal-cancel');

    title.textContent   = opts.title   || 'Are you sure?';
    message.textContent = opts.message || '';
    btnOk.textContent   = opts.ok      || 'Confirm';
    btnCan.textContent  = opts.cancel  || 'Cancel';

    btnOk.className = 'fu-modal-ok btn ' + (opts.variant === 'warning' ? 'btn-warning' : 'btn-danger');
    icon.className  = 'fu-modal-icon fu-modal-icon--' + (opts.variant || 'danger');

    if (opts.variant === 'warning') {
      icon.innerHTML = '<svg width="24" height="24" viewBox="0 0 24 24" fill="none"><path d="M12 9v4M12 17h.01M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>';
    } else {
      icon.innerHTML = '<svg width="24" height="24" viewBox="0 0 24 24" fill="none"><path d="M12 22c5.523 0 10-4.477 10-10S17.523 2 12 2 2 6.477 2 12s4.477 10 10 10z" stroke="currentColor" stroke-width="2"/><path d="M12 8v4M12 16h.01" stroke="currentColor" stroke-width="2" stroke-linecap="round"/></svg>';
    }

    modal.classList.add('is-open');
    btnCan.focus();

    btnOk.onclick = function () {
      closeModal();
      if (typeof opts.onConfirm === 'function') opts.onConfirm();
    };
  };

  function closeModal() {
    const modal = getModal();
    if (modal) modal.classList.remove('is-open');
    _pendingForm = null;
  }

  // Intercept all forms with data-confirm attribute
  document.addEventListener('submit', function (e) {
    const form = e.target;
    if (!form || !form.matches('form[data-confirm]')) return;

    const msg     = form.dataset.confirm;
    const title   = form.dataset.confirmTitle   || 'Confirm action';
    const ok      = form.dataset.confirmOk      || 'Yes, proceed';
    const variant = form.dataset.confirmVariant || 'danger';

    e.preventDefault();
    e.stopImmediatePropagation();

    const capturedForm = form;

    fuConfirm({
      title, message: msg, ok, variant,
      onConfirm: function () {
        capturedForm.removeAttribute('data-confirm');
        requestAnimationFrame(function () { capturedForm.submit(); });
      }
    });
  }, true);

  document.addEventListener('click',   function (e) { if (e.target.closest('.fu-modal-cancel') || e.target === getModal()) closeModal(); });
  document.addEventListener('keydown', function (e) { if (e.key === 'Escape') closeModal(); });
})();


// ── TOAST NOTIFICATION ───────────────────────────────────────────
window.fuToast = (function () {
  function show(message, type, duration) {
    type = type || 'success'; duration = duration || 3500;
    let rack = document.getElementById('fu-toast-rack');
    if (!rack) { rack = document.createElement('div'); rack.id = 'fu-toast-rack'; document.body.appendChild(rack); }

    const icons = {
      success: '<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><circle cx="9" cy="9" r="8" stroke="currentColor" stroke-width="1.5"/><path d="M5.5 9l2.5 2.5 4.5-4.5" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/></svg>',
      error:   '<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><circle cx="9" cy="9" r="8" stroke="currentColor" stroke-width="1.5"/><path d="M6 6l6 6M12 6l-6 6" stroke="currentColor" stroke-width="1.8" stroke-linecap="round"/></svg>',
      info:    '<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><circle cx="9" cy="9" r="8" stroke="currentColor" stroke-width="1.5"/><path d="M9 8v5M9 6h.01" stroke="currentColor" stroke-width="1.8" stroke-linecap="round"/></svg>',
      warning: '<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><path d="M9 2L1 16h16L9 2z" stroke="currentColor" stroke-width="1.5" stroke-linejoin="round"/><path d="M9 8v3M9 13h.01" stroke="currentColor" stroke-width="1.8" stroke-linecap="round"/></svg>',
    };

    const t = document.createElement('div');
    t.className = 'fu-toast fu-toast--' + type;
    t.innerHTML = '<span class="fu-toast-icon">' + (icons[type]||icons.info) + '</span>'
                + '<span class="fu-toast-msg">' + message + '</span>'
                + '<button class="fu-toast-close" aria-label="Close"><svg width="14" height="14" viewBox="0 0 14 14"><path d="M2 2l10 10M12 2L2 12" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg></button>';
    rack.appendChild(t);

    requestAnimationFrame(() => requestAnimationFrame(() => t.classList.add('is-visible')));
    const timer = setTimeout(() => dismiss(t), duration);
    t.querySelector('.fu-toast-close').addEventListener('click', function () { clearTimeout(timer); dismiss(t); });
  }

  function dismiss(el) {
    el.classList.remove('is-visible');
    el.addEventListener('transitionend', () => el.remove(), { once: true });
  }

  return { show };
})();


// ── COMMENT MENU TOGGLE ──────────────────────────────────────────
window.toggleCommentMenu = function (id, e) {
  e && e.stopPropagation();
  const menu = document.getElementById('cmenu-' + id);
  if (!menu) return;
  const isOpen = menu.classList.contains('is-open');
  document.querySelectorAll('.comment-menu-dropdown.is-open').forEach(m => m.classList.remove('is-open'));
  if (!isOpen) menu.classList.add('is-open');
};
document.addEventListener('click', function () {
  document.querySelectorAll('.comment-menu-dropdown.is-open').forEach(m => m.classList.remove('is-open'));
});

// ── COMMENT EDIT ─────────────────────────────────────────────────
window.startEditComment = function (id) {
  const ctext = document.getElementById('ctext-' + id);
  if (ctext) ctext.classList.add('d-none');
  const cedit = document.getElementById('cedit-' + id);
  if (cedit) { cedit.style.display = ''; cedit.querySelector('textarea')?.focus(); }
  document.getElementById('cmenu-' + id)?.classList.remove('is-open');
};

window.cancelEditComment = function (id) {
  document.getElementById('ctext-' + id)?.classList.remove('d-none');
  const cedit = document.getElementById('cedit-' + id);
  if (cedit) cedit.style.display = 'none';
};

// ── REPLY TOGGLE ─────────────────────────────────────────────────
document.addEventListener('click', function (e) {
  const btn = e.target.closest('.fb-reply-toggle');
  if (!btn) return;
  const target = document.getElementById(btn.dataset.replyTarget);
  if (!target) return;
  const isOpen = target.classList.contains('is-open');
  document.querySelectorAll('.fb-reply-form.is-open').forEach(f => f.classList.remove('is-open'));
  if (!isOpen) { target.classList.add('is-open'); target.querySelector('input')?.focus(); }
});
