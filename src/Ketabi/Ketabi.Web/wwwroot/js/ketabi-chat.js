/**
 * ketabi-chat.js
 * UI behaviour + SignalR real-time chat
 */

(function () {
    'use strict';

    // Helpers
    function qs(sel, root) { return (root || document).querySelector(sel); }
    function qsa(sel, root) { return Array.from((root || document).querySelectorAll(sel)); }

    function debounce(fn, ms) {
        var t;
        return function () {
            clearTimeout(t);
            t = setTimeout(fn, ms);
        };
    }

    // Auto-scroll
    function initFeedScroll() {
        var end = qs('#chat-feed-end');
        if (end) end.scrollIntoView({ behavior: 'instant', block: 'end' });
    }

    function scrollToBottom() {
        var end = qs('#chat-feed-end');
        if (end) end.scrollIntoView({ behavior: 'smooth', block: 'end' });
    }

    // Message input: enable send + auto-grow
    function initMessageInput() {
        var textarea = qs('#chat-input');
        var sendBtn  = qs('#chat-send-btn');
        if (!textarea || !sendBtn) return;

        function updateSendBtn() {
            sendBtn.disabled = textarea.value.trim().length === 0;
        }

        function autoGrow() {
            textarea.style.height = 'auto';
            textarea.style.height = Math.min(textarea.scrollHeight, 120) + 'px';
        }

        textarea.addEventListener('input', function () {
            updateSendBtn();
            autoGrow();
        });

        updateSendBtn();
        autoGrow();
    }

    // ─── BUG 6 FIX: Handoff bar collapse/expand ────────────────────────────────
    // Root cause: after collapse sets max-height:0 + overflow:hidden, re-expanding
    // read scrollHeight which returns 0, so max-height was set to 0px again.
    // Fix: capture naturalHeight before any collapse so it is always available.
    function initHandoffToggle() {
        var toggle  = qs('#handoff-toggle');
        var content = qs('#handoff-content');
        if (!toggle || !content) return;

        var naturalHeight = 0;

        // Measure the natural (unconstrained) height before applying any collapse
        content.style.overflow  = 'visible';
        content.style.maxHeight = 'none';
        naturalHeight = content.scrollHeight;
        content.style.maxHeight  = naturalHeight + 'px';
        content.style.overflow   = '';
        content.style.transition = 'max-height 0.3s ease';

        toggle.addEventListener('click', function () {
            var expanded = toggle.getAttribute('aria-expanded') === 'true';
            if (expanded) {
                content.style.maxHeight = '0px';
                content.style.overflow  = 'hidden';
                toggle.setAttribute('aria-expanded', 'false');
            } else {
                // Re-measure in case DOM changed (e.g. confirmation badge added);
                // fall back to the captured naturalHeight if scrollHeight is still 0.
                content.style.overflow  = 'visible';
                naturalHeight = content.scrollHeight || naturalHeight;
                content.style.maxHeight = naturalHeight + 'px';
                setTimeout(function () {
                    content.style.overflow = '';
                }, 310);
                toggle.setAttribute('aria-expanded', 'true');
            }
        });
    }

    // Sidebar search
    function initSidebarSearch() {
        var input = qs('#chat-search');
        var list  = qs('#conversation-list');
        if (!input || !list) return;

        input.addEventListener('input', function () {
            var q = input.value.toLowerCase().trim();
            qsa('.chat-conv-item', list).forEach(function (item) {
                var name = (item.dataset.searchName || '').toLowerCase();
                var book = (item.dataset.searchBook || '').toLowerCase();
                item.style.display = (!q || name.includes(q) || book.includes(q)) ? '' : 'none';
            });
        });
    }

    // Star rating
    function initStarRating() {
        var starBtns    = qsa('.review-card__star-btn');
        var hiddenInput = qs('#rating-hidden-input');
        var ratingLabel = qs('#rating-label');
        var submitBtn   = qs('#review-submit-btn');
        if (!starBtns.length || !hiddenInput) return;

        var labels   = ['', 'Poor', 'Fair', 'Good', 'Great', 'Excellent!'];
        var selected = 0;

        function renderStars(hover) {
            var active = hover || selected;
            starBtns.forEach(function (btn) {
                var v    = parseInt(btn.dataset.value, 10);
                var icon = btn.querySelector('.review-card__star-icon');
                icon.className = v <= active
                    ? 'bi bi-star-fill review-card__star-icon'
                    : 'bi bi-star review-card__star-icon';
            });
        }

        starBtns.forEach(function (btn) {
            btn.addEventListener('mouseenter', function () { renderStars(parseInt(btn.dataset.value, 10)); });
            btn.addEventListener('mouseleave', function () { renderStars(0); });
            btn.addEventListener('click', function () {
                selected = parseInt(btn.dataset.value, 10);
                hiddenInput.value = selected;
                if (ratingLabel) ratingLabel.textContent = labels[selected] || '';
                if (submitBtn)   submitBtn.disabled = selected === 0;
                renderStars(0);
            });
        });
    }

    // ─── BUG 1 FIX: SignalR real-time chat ────────────────────────────────────
    // Guard prevents initSignalR() from registering duplicate connection.on() handlers
    // if it is somehow called more than once. connection.on('ReceiveMessage', ...) is
    // registered exactly once; appendMessage() is called exclusively inside that handler
    // (no optimistic local append).
    function initSignalR() {
        var layout = qs('.chat-layout');
        if (!layout || layout.dataset.signalrInit === 'true') return;
        layout.dataset.signalrInit = 'true';

        var conversationId = layout.dataset.activeConv;
        if (!conversationId) return;

        var feed     = qs('#chat-feed');
        var textarea = qs('#chat-input');
        var sendBtn  = qs('#chat-send-btn');

        // ─── BUG 8 FIX: read currentUserId from the data attribute ──────────
        var currentUserId = layout.dataset.currentUserId || '';

        // Build connection
        var connection = new signalR.HubConnectionBuilder()
            .withUrl('/hubs/chat')
            .withAutomaticReconnect()
            .build();

        // ─── BUG 3 FIX: ReceiveMessage — determine ownership client-side ────
        // C: appendMessage() is called ONLY here; no optimistic append elsewhere.
        connection.on('ReceiveMessage', function (msg) {
            msg.isOwn = (String(msg.senderId || msg.senderID || msg.sender || '') === String(currentUserId));
            appendMessage(msg);
            scrollToBottom();

            if (document.hasFocus()) {
                connection.invoke('MarkRead', conversationId).catch(console.error);
            }
        });

        // Event: typing indicator
        connection.on('UserTyping', function () {
            showTypingIndicator();
        });

        // Event: messages read
        connection.on('MessagesRead', function () {
            // TODO: update read receipt UI
        });

        // Event: handoff confirmed
        connection.on('HandoffConfirmed', function () {
            window.location.reload();
        });

        // Event: error
        connection.on('Error', function (message) {
            console.error('ChatHub error:', message);
        });

        // Start + join group
        async function start() {
            try {
                await connection.start();
                await connection.invoke('JoinConversation', conversationId);
            } catch (err) {
                console.error('SignalR connection error:', err);
                setTimeout(start, 3000);
            }
        }
        start();

        // Reconnection: re-join group
        connection.onreconnected(function () {
            connection.invoke('JoinConversation', conversationId).catch(console.error);
        });

        // Send message
        if (sendBtn && textarea) {
            sendBtn.addEventListener('click', sendMessage);
            textarea.addEventListener('keydown', function (e) {
                if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
                    e.preventDefault();
                    if (!sendBtn.disabled) sendMessage();
                }
            });

            // Typing indicator — debounced
            textarea.addEventListener('input', debounce(function () {
                if (connection.state === signalR.HubConnectionState.Connected) {
                    connection.invoke('Typing', conversationId).catch(console.error);
                }
            }, 300));
        }

        function sendMessage() {
            var text = textarea.value.trim();
            if (!text) return;

            textarea.value       = '';
            sendBtn.disabled     = true;
            textarea.style.height = 'auto';

            connection.invoke('SendMessage', conversationId, text)
                .catch(function (err) {
                    console.error('Send error:', err);
                    textarea.value   = text; // restore on fail
                    sendBtn.disabled = false;
                })
                .finally(function () {
                    sendBtn.disabled = textarea.value.trim().length === 0;
                    textarea.focus();
                });
        }

        // Mark read on window focus
        window.addEventListener('focus', function () {
            if (connection.state === signalR.HubConnectionState.Connected) {
                connection.invoke('MarkRead', conversationId).catch(console.error);
            }
        });

        // ─── BUG 3 FIX: appendMessage — strict null guard ───────────────────
        // anchor and feed must both exist; never fall back to document.body.
        // textContent used for the bubble text — XSS safe, no innerHTML for user content.
        function appendMessage(msg) {
            removeTypingIndicator();

            var anchor = qs('#chat-feed-end');
            if (!anchor || !feed) return;   // strict null guard — never append to body

            var isMine = msg.isOwn;
            var time   = new Date(msg.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

            var row = document.createElement('div');
            row.className    = 'chat-bubble-row ' + (isMine ? 'chat-bubble-row--mine' : 'chat-bubble-row--theirs');
            row.dataset.msgId = msg.messageId;

            if (!isMine) {
                var avatar = document.createElement('img');
                // Resolve avatar URL robustly: DTO may carry filename, relative path, or full URL.
                var rawAvatar = msg.senderAvatar || msg.senderAvatarUrl || msg.senderAvatarURL || '';
                var avatarSrc = '/uploads/users/default-avatar.png';
                try {
                    if (rawAvatar && rawAvatar.length > 0) {
                        // Normalize tilde paths
                        if (rawAvatar.startsWith('~')) rawAvatar = rawAvatar.replace(/^~\/?/, '/');

                        // If it already contains uploads/users, trust it
                        if (rawAvatar.indexOf('/uploads/users/') !== -1) {
                            avatarSrc = rawAvatar.startsWith('/') ? rawAvatar : '/' + rawAvatar;
                        }
                        else if (rawAvatar.startsWith('/') || rawAvatar.startsWith('http')) {
                            avatarSrc = rawAvatar;
                        } else {
                            // treat as filename
                            avatarSrc = '/uploads/users/' + rawAvatar;
                        }
                    }
                } catch (e) {
                    avatarSrc = '/uploads/users/default-avatar.png';
                }

                // Set up fallback when image fails to load (e.g., not yet available)
                avatar.src = avatarSrc;
                avatar.alt = msg.senderName || '';
                avatar.className = 'chat-bubble-row__avatar';
                avatar.loading = 'lazy';
                avatar.onerror = function () {
                    // Avoid infinite loop if default also fails
                    if (this.src && !this.src.endsWith('/default-avatar.png')) {
                        this.src = '/uploads/users/default-avatar.png';
                    }
                };

                // When the image is a local file that may not yet be present (recent upload),
                // append a cache-busting query parameter to force browser revalidation.
                // Use origin-less param only when filename (no slash) to avoid breaking CDN urls.
                if (rawAvatar && rawAvatar.indexOf('/') === -1 && !avatar.src.includes('?')) {
                    avatar.src = avatar.src + '?v=' + Date.now();
                }

                row.appendChild(avatar);
            }

            var group  = document.createElement('div');
            group.className = 'chat-bubble-group';

            var bubble = document.createElement('div');
            bubble.className = 'chat-bubble ' + (isMine ? 'chat-bubble--mine' : 'chat-bubble--theirs');
            bubble.textContent = msg.text; // textContent — XSS safe, never innerHTML for user content

            var timeEl = document.createElement('span');
            timeEl.className   = 'chat-bubble-group__time';
            timeEl.textContent = time;
            // BUG 7: dynamically appended messages always show their timestamp
            // (grouping applies only to server-rendered history)

            group.appendChild(bubble);
            group.appendChild(timeEl);
            row.appendChild(group);

            feed.insertBefore(row, anchor);
        }

        // ─── BUG 3 FIX: showTypingIndicator — strict null guard + no innerHTML ──
        var typingTimer;

        function showTypingIndicator() {
            removeTypingIndicator();

            var anchor = qs('#chat-feed-end');
            if (!anchor || !feed) return;   // strict null guard — never append to body

            // Build DOM elements manually — no innerHTML, no user content involved
            var indicator    = document.createElement('div');
            indicator.id     = 'typing-indicator';
            indicator.className = 'chat-bubble-row chat-bubble-row--theirs';

            var group  = document.createElement('div');
            group.className = 'chat-bubble-group';

            var bubble = document.createElement('div');
            bubble.className = 'chat-bubble chat-bubble--theirs chat-bubble--typing';

            for (var d = 0; d < 3; d++) {
                var dot = document.createElement('span');
                dot.className = 'typing-dot';
                bubble.appendChild(dot);
            }

            group.appendChild(bubble);
            indicator.appendChild(group);

            feed.insertBefore(indicator, anchor);
            scrollToBottom();

            typingTimer = setTimeout(removeTypingIndicator, 3000);
        }

        function removeTypingIndicator() {
            clearTimeout(typingTimer);
            var existing = qs('#typing-indicator');
            if (existing) existing.remove();
        }
    }

    // Review modal close handler
    function initReviewModal() {
        var closeBtn = document.querySelector('#review-modal-close');
        if (!closeBtn) return;
        closeBtn.addEventListener('click', function () {
            var modal = document.querySelector('#review-modal');
            if (modal) modal.style.display = 'none';
        });
    }

    // Boot
    function init() {
        initFeedScroll();
        initMessageInput();
        initHandoffToggle();
        initSidebarSearch();
        initStarRating();
        initReviewModal();
        initSignalR();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();