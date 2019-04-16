// mutation observer
$(document).ready(function () {
    var mutationObserver = window.MutationObserver || window.WebKitMutationObserver,
        invoke = function (eventType, nodes) {
            $.trigger.call(document, eventType, { nodes: nodes });
        }

    if (mutationObserver) {
        var observer = new mutationObserver(function (mutations, observer) {
            if (mutations[0].addedNodes.length)
                invoke("dom.insert", mutations[0].addedNodes);
            if (mutations[0].removedNodes.length)
                invoke("dom.remove", mutations[0].removedNodes);
        });
        observer.observe(obj, { childList: true, subtree: true });
    }
    else if (window.addEventListener) {
        document.addEventListener('DOMNodeInserted', function (e) { invoke("dom.insert", [e.target]); }, false);
        document.addEventListener('DOMNodeRemoved', function (e) { invoke("dom.remove", [e.target]); }, false);
    }
})


$.fn.nodesAdded = function (callback) {
    var mutationObserver = window.MutationObserver || window.WebKitMutationObserver;

    if (mutationObserver) {
        var observer = new mutationObserver(function (mutations, observer) {
            if (mutations[0].addedNodes.length)
                callback.call(this);
        });
        observer.observe(this[0], { childList: true, subtree: true });
    }
    else if (window.addEventListener) {
        this[0].addEventListener('DOMNodeInserted', callback, false);
    }
}

$.fn.nodesRemoved = function (callback) {
    var mutationObserver = window.MutationObserver || window.WebKitMutationObserver;

    if (mutationObserver) {
        var observer = new mutationObserver(function (mutations, observer) {
            if (mutations[0].removedNodes.length)
                callback.call(this);
        });
        observer.observe(this[0], { childList: true, subtree: true });
    }
    else if (window.addEventListener) {
        this[0].addEventListener('DOMNodeRemoved', callback, false);
    }
}
