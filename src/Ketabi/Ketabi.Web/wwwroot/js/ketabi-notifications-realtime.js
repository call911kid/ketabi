(function () {
    'use strict';

    // Abort silently on unauthenticated pages where the bell is absent.
    const bellBtn = document.getElementById('notif-bell-btn');
    if (!bellBtn) return;

    // ── Connection ────────────────────────────────────────────────────────────
    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/hubs/notifications')
        .withAutomaticReconnect([2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Warning)
        .build();

    connection.on('notification:received', function (dto) {
        incrementBellBadge();
        showToast(dto);
        prependNotificationCard(dto);
    });

    connection.start().catch(function (err) {
        // Non-critical — the user still sees all notifications on page load.
        console.warn('[Ketabi] SignalR connection failed:', err);
    });

    // ── Bell Badge ────────────────────────────────────────────────────────────
    function incrementBellBadge() {
        let badge = document.getElementById('notif-bell-badge');

        if (!badge) {
            badge = document.createElement('span');
            badge.id = 'notif-bell-badge';
            badge.className = 'notif-nav-badge';
            badge.textContent = '1';
            bellBtn.appendChild(badge);
            return;
        }

        const current = parseInt(badge.textContent, 10) || 0;
        badge.textContent = current + 1;
    }

    // ── Toast ─────────────────────────────────────────────────────────────────
    function showToast(dto) {
        console.log("Toast Now");
        //("Alert from Toast");
        const container = getOrCreateToastContainer();
        const toast     = buildToast(dto);

        container.appendChild(toast);

        requestAnimationFrame(function () {
            toast.classList.add('notif-toast--visible');
        });

        const dismissTimer = setTimeout(function () { dismissToast(toast); }, 5000);

        toast.querySelector('.notif-toast__close').addEventListener('click', function () {
            clearTimeout(dismissTimer);
            dismissToast(toast);
        });
    }

    function getOrCreateToastContainer() {
        let container = document.getElementById('notif-toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'notif-toast-container';
            container.className = 'notif-toast-container';
            document.body.appendChild(container);
        }
        return container;
    }

    function buildToast(dto) {
        const toast = document.createElement('div');
        toast.className = 'notif-toast';
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'polite');

        toast.innerHTML =
            '<div class="notif-toast__icon">' + getTypeIcon(dto.notificationType) + '</div>' +
            '<div class="notif-toast__body">' +
                '<p class="notif-toast__title">' + escapeHtml(dto.title) + '</p>' +
                '<p class="notif-toast__content">' + escapeHtml(dto.content) + '</p>' +
            '</div>' +
            '<button class="notif-toast__close" aria-label="Dismiss">' +
                '<i class="bi bi-x"></i>' +
            '</button>';

        return toast;
    }

    function dismissToast(toast) {
        toast.classList.remove('notif-toast--visible');
        toast.classList.add('notif-toast--exit');
        toast.addEventListener('transitionend', function () { toast.remove(); }, { once: true });
    }

    // ── Live Card (Notifications Page Only) ───────────────────────────────────
    function prependNotificationCard(dto) {
        const list = document.getElementById('notif-live-list');
        if (!list) return;

        const emptyState = list.querySelector('.notif-empty');
        if (emptyState) emptyState.remove();

        const card = buildNotificationCard(dto);
        list.insertBefore(card, list.firstChild);

        requestAnimationFrame(function () {
            card.classList.add('notif-card--live-enter');
        });
    }

    function buildNotificationCard(dto) {
        const card = document.createElement('div');
        card.className = 'notif-card notif-card--unread';

        card.innerHTML =
            '<span class="notif-card__dot"></span>' +
            '<div class="notif-card__icon ' + getTypeIconClass(dto.notificationType) + '">' +
                getTypeIcon(dto.notificationType) +
            '</div>' +
            '<div class="notif-card__body">' +
                '<p class="notif-card__title">' + escapeHtml(dto.title) + '</p>' +
                '<p class="notif-card__content">' + escapeHtml(dto.content) + '</p>' +
            '</div>' +
            '<span class="notif-card__time">just now</span>';

        return card;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    // NotificationType enum: General=0, RequestUpdate=1, Review=2, Message=3, System=4
    function getTypeIcon(type) {
        const icons = {
            0: '<i class="bi bi-bell"></i>',
            1: '<i class="bi bi-arrow-left-right"></i>',
            2: '<i class="bi bi-star"></i>',
            3: '<i class="bi bi-chat"></i>',
            4: '<i class="bi bi-gear"></i>'
        };
        return icons[type] || icons[0];
    }

    function getTypeIconClass(type) {
        const classes = {
            0: 'notif-card__icon--general',
            1: 'notif-card__icon--request',
            2: 'notif-card__icon--review',
            3: 'notif-card__icon--message',
            4: 'notif-card__icon--system'
        };
        return classes[type] || classes[0];
    }

    function escapeHtml(str) {
        if (!str) return '';
        return str
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

}());
