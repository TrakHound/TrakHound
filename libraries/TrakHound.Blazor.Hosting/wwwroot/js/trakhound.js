var stored_timers = [];


function Timer(timerId, timerElement, options) {

    var offset,
        clock,
        interval;

    // default options
    options = options || {};
    options.delay = options.delay || 10;

    // initialize
    reset();

    function start() {
        if (!interval) {
            offset = Date.now();
            interval = setInterval(update, options.delay);
        }
    }

    function stop() {
        if (interval) {
            clearInterval(interval);
            interval = null;
        }
    }

    function reset() {
        clock = 0;
        render(0);
    }

    function update() {
        clock += delta();
        render();
    }

    function render() {
        timerElement.innerHTML = Number(clock / 1000).toFixed(3) + 's';
    }

    function delta() {
        var now = Date.now(),
            d = now - offset;

        offset = now;
        return d;
    }

    // public API
    this.start = start;
    this.stop = stop;
    this.reset = reset;

    stored_timers[timerId] = this;

};


var JsFunctions = window.JsFunctions || {};
JsFunctions = {
    focusElement: function (id) {
        const element = document.getElementById(id); element.focus();
    },
    clickElement: function (id) {
        const element = document.getElementById(id); element.click();
    },
    getElementWidth: function (id, position) {
        const element = document.getElementById(id);
        return element.getBoundingClientRect().width;
    },
    scrollIntoView: function (id) {
        const element = document.getElementById(id);
        element.scrollIntoView();
    },
    setCursorPosition: function (id, position) {
        const element = document.getElementById(id);
        element.setSelectionRange(position, position);
    },
    limitCursorPosition: function (id, limit) {
        const element = document.getElementById(id);
        if (element.selectionStart <= limit) {
            element.setSelectionRange(limit, limit);
        }
    },
    startTimer: function (timerId, timerElementId) {
        const timerElement = document.getElementById(timerElementId);
        var timer = new Timer(timerId, timerElement);
        timer.start();
    },
    stopTimer: function (timerId) {
        var timer = stored_timers[timerId];
        timer.stop();
    },
    copyClipboard: function (text) {

        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(text);
        } else {
            const textarea = document.createElement('textarea');
            textarea.value = text;

            // Move the textarea outside the viewport to make it invisible
            textarea.style.position = 'absolute';
            textarea.style.left = '-99999999px';

            document.body.prepend(textarea);

            // highlight the content of the textarea element
            textarea.select();

            try {
                document.execCommand('copy');
            } catch (err) {
                console.log(err);
            } finally {
                textarea.remove();
            }
        }
    }
};