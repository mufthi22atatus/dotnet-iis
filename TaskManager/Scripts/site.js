// Tiny client helpers; the Web API is the interesting surface for AJAX integrations.
(function ($) {
    'use strict';

    // Auto-dismiss alerts after 5s
    setTimeout(function () {
        document.querySelectorAll('.alert').forEach(function (el) {
            el.classList.add('fade');
            setTimeout(function () { el.remove(); }, 300);
        });
    }, 5000);

    // Example: convenience wrapper for Web API calls with anti-forgery + JSON
    window.TaskApi = {
        listTasks: function (includeDone) {
            return fetch('/api/tasks?includeDone=' + (includeDone ? 'true' : 'false'), {
                credentials: 'same-origin',
                headers: { 'Accept': 'application/json' }
            }).then(function (r) { return r.json(); });
        },
        setStatus: function (id, status) {
            return fetch('/api/tasks/' + id + '/status', {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
                body: JSON.stringify({ status: status })
            }).then(function (r) { return r.json(); });
        }
    };
})(window.jQuery);
