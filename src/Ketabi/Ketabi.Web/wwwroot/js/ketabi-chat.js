/**
 * ketabi-chat.js
 * ─────────────────────────────────────────────────────────────────────────────
 * Chat page interactive behaviour
 *   1. Auto-scroll feed to bottom on load
 *   2. Send-button enable/disable based on textarea content
 *   3. Auto-grow textarea height as user types
 *   4. Handoff bar collapse/expand toggle
 *   5. Sidebar conversation search filter (client-side)
 *   6. Star rating interaction for the review form
 * ─────────────────────────────────────────────────────────────────────────────
 */

(function () {
  'use strict';

  /* ── Helpers ─────────────────────────────────────────────────────────── */
  function qs(selector, root) { return (root || document).querySelector(selector); }
  function qsa(selector, root) { return Array.from((root || document).querySelectorAll(selector)); }

  /* ── 1. Auto-scroll chat feed to bottom ─────────────────────────────── */
  function initFeedScroll() {
    var end = qs('#chat-feed-end');
    if (end) {
      end.scrollIntoView({ behavior: 'instant', block: 'end' });
    }
  }

  /* ── 2 & 3. Message input: enable send + auto-grow ──────────────────── */
  function initMessageInput() {
    var textarea = qs('#chat-input');
    var sendBtn  = qs('#chat-send-btn');
    if (!textarea || !sendBtn) return;

    function updateSendBtn() {
      var hasText = textarea.value.trim().length > 0;
      sendBtn.disabled = !hasText;
    }

    function autoGrow() {
      textarea.style.height = 'auto';
      var maxH = 120;
      textarea.style.height = Math.min(textarea.scrollHeight, maxH) + 'px';
    }

    textarea.addEventListener('input', function () {
      updateSendBtn();
      autoGrow();
    });

    // Allow Ctrl+Enter or Cmd+Enter to submit (optional UX enhancement)
    textarea.addEventListener('keydown', function (e) {
      if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
        e.preventDefault();
        if (!sendBtn.disabled) {
          sendBtn.closest('form') && sendBtn.closest('form').submit();
        }
      }
    });

    // Initial state
    updateSendBtn();
    autoGrow();
  }

  /* ── 4. Handoff bar collapse/expand ─────────────────────────────────── */
  function initHandoffToggle() {
    var toggle  = qs('#handoff-toggle');
    var content = qs('#handoff-content');
    if (!toggle || !content) return;

    // Set initial max-height so transition works
    content.style.maxHeight = content.scrollHeight + 'px';

    toggle.addEventListener('click', function () {
      var expanded = toggle.getAttribute('aria-expanded') === 'true';

      if (expanded) {
        // Collapse
        content.style.maxHeight = content.scrollHeight + 'px'; // snapshot
        requestAnimationFrame(function () {
          content.style.maxHeight = '0';
          content.style.overflow  = 'hidden';
          content.style.paddingBottom = '0';
        });
        toggle.setAttribute('aria-expanded', 'false');
      } else {
        // Expand
        content.style.maxHeight  = content.scrollHeight + 'px';
        content.style.overflow   = '';
        content.style.paddingBottom = '';
        toggle.setAttribute('aria-expanded', 'true');
      }
    });
  }

  /* ── 5. Sidebar search filter (client-side) ─────────────────────────── */
  function initSidebarSearch() {
    var searchInput = qs('#chat-search');
    var list        = qs('#conversation-list');
    if (!searchInput || !list) return;

    searchInput.addEventListener('input', function () {
      var query = searchInput.value.toLowerCase().trim();
      var items = qsa('.chat-conv-item', list);

      items.forEach(function (item) {
        var name = (item.dataset.searchName || '').toLowerCase();
        var book = (item.dataset.searchBook || '').toLowerCase();
        var matches = !query || name.includes(query) || book.includes(query);
        item.style.display = matches ? '' : 'none';
      });
    });
  }

  /* ── 6. Star rating ──────────────────────────────────────────────────── */
  function initStarRating() {
    var starBtns     = qsa('.review-card__star-btn');
    var hiddenInput  = qs('#rating-hidden-input');
    var ratingLabel  = qs('#rating-label');
    var submitBtn    = qs('#review-submit-btn');

    if (!starBtns.length || !hiddenInput) return;

    var labels = ['', 'Poor', 'Fair', 'Good', 'Great', 'Excellent!'];
    var selectedRating = 0;

    function renderStars(hoverValue) {
      var active = hoverValue || selectedRating;
      starBtns.forEach(function (btn) {
        var v = parseInt(btn.dataset.value, 10);
        var icon = btn.querySelector('.review-card__star-icon');
        if (v <= active) {
          icon.className = 'bi bi-star-fill review-card__star-icon';
          btn.classList.add('is-active');
        } else {
          icon.className = 'bi bi-star review-card__star-icon';
          btn.classList.remove('is-active');
        }
      });
    }

    starBtns.forEach(function (btn) {
      btn.addEventListener('mouseenter', function () {
        renderStars(parseInt(btn.dataset.value, 10));
      });

      btn.addEventListener('mouseleave', function () {
        renderStars(0); // revert to selected
      });

      btn.addEventListener('click', function () {
        selectedRating = parseInt(btn.dataset.value, 10);
        hiddenInput.value = selectedRating;

        if (ratingLabel) {
          ratingLabel.textContent = labels[selectedRating] || '';
        }

        if (submitBtn) {
          submitBtn.disabled = selectedRating === 0;
        }

        renderStars(0);
      });
    });
  }

  /* ── Boot ────────────────────────────────────────────────────────────── */
  function init() {
    initFeedScroll();
    initMessageInput();
    initHandoffToggle();
    initSidebarSearch();
    initStarRating();
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }

})();
