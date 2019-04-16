$.bind = function (template, data) {
    var carets = template.match(/\{\{[\w\.\$\-]+\}\}/g);

    for (var i = 0; carets && i < carets.length; i++) {
        var expr = carets[i].slice(2, -2).split("."), val = data;

        try {
            for (var key in expr) {
                val = val[expr[key]];
            }
            if (val === null || val === undefined) {
                val = "";
            }
        }
        catch (err) {
            val = "!ref#error!";
        };
        
        template = template.replace(carets[i], val);
    }
    return template;
}

$.dialog = function (url, params, validation) {
    var $modal = $('<div class="modal"></div>'),
        fn_fail, fn_done, fn_always, fn_load, fn_close;

    var removeModal = function () {
        $modal.remove();
        if ($(".modal:visible").length == 0)
            $(document.body).removeClass("modal-open");
    }

    var task = $modal.load(url, params)
        .done(function () {
            var closeModal = true, result;

            $modal.show();
            $modal.find(".modal-button").on("click", function (event) {

                if (validation) {
                    if (!validation.call($modal[0], this.name))
                        return;
                }

                if (fn_done) 
                    result = fn_done.call($modal[0], this.name);

                if (fn_close) 
                    result = fn_close.call($modal[0], this);

                $.isPromise(result)
                    ? result.done(removeModal)
                    : removeModal();
            });
            
            if (fn_load) fn_load.apply($modal[0], arguments);
        })
        .fail(function () {
            if (fn_fail) fn_fail.apply($modal[0], arguments);
        })
        .always(function () {
            if (fn_always) fn_always.apply($modal[0], arguments);
        })

    $(document.body).append($modal.css({display: "block"}));
    $(document.body).addClass("modal-open");

   

    // builder
    var builder = {
        data: function(data){
            $modal.data($.extend($modal.data(), data)); return builder;
        },
        done: function (callback) {
            fn_done = callback; return builder;
        },
        always: function (callback) {
            fn_always = callback; return builder;
        },
        fail: function (callback) {
            fn_fail = callback; return builder;
        },
        load: function (callback) {
            fn_load = callback; return builder;
        },
        close: function (callback) {
            fn_close = callback; return builder;
        }
    }

    //api
    $modal.data({
        dialog: {
            builder: builder,
            close: removeModal
        }
    });

    return builder;
}

$.fn.dialog = function () {
    var $dialog = this.is(".modal") ? this : this.parents(".modal:first");

    // return the api
    return ($dialog.data() || {}).dialog;
}

$.toast = function (options) {
    var defaults = {
            icon: "fa fa-info",
            title: "Notice",
            message: ""
        },
        template = '<div class="toast">'
                 +   '<div class="toast-title">'
                 +     '<i class="{{icon}}"></i><span>{{title}}</span>'
                 +   '</div>'
                 +   '<p>{{message}}</p>'
                 + '</div>',

        html = $.bind(template, $.extend(defaults, options)),
        $node = $(html).prependTo($("body"));

    window.setTimeout(function () { $node.remove(); }, 2000);
}

$.fn.overlay = function (url, params) {
    var $target = this, $layer = $("<div/>");

    $layer.addClass("layer").css({
        backgroundColor: $target.css("background-color")
    });

    $("<div/>").addClass("layer-body").appendTo($layer);

    $layer.resize = function () {
        $layer.offset($target.offset()).css({
            height: $target.outerHeight(true) + "px",
            width: $target.outerWidth(true) + "px"
        });

        return $layer;
    }

    $layer.open = function () {
        return $layer.resize().removeClass("layer-out").addClass("layer-in");
    }

    $layer.close = function () {
        return $layer.removeClass("layer-in").addClass("layer-out");
    }

    return $layer
        .appendTo(document.body)
        .children()
        .load(url, params)
        .done(function () {
            $("<div><a>&times;</a></div>")
                .addClass("layer-close")
                .appendTo($layer.children())
                .click(function () {
                    $layer.close();
                });

            $layer.open();
        });
}

$.fn.slideInRight = function () {
    var $target = this, $layer = $("<div/>");

    $layer.addClass("layer").css({
        backgroundColor: $target.css("background-color")
    });

    $("<div/>").addClass("layer-body").appendTo($layer);

    $layer.resize = function () {
        $layer.offset($target.offset()).css({
            height: $target.outerHeight(true) + "px",
            width: $target.outerWidth(true) + "px"
        });

        return $layer;
    }

    $layer.open = function () {
        return $layer.resize().removeClass("layer-out").addClass("layer-in");
    }

    $layer.close = function () {
        return $layer.removeClass("layer-in").addClass("layer-out");
    }

    return $layer
        .appendTo(document.body)
        .children()
        .load(url, params)
        .done(function () {
            $("<div><a>&times;</a></div>")
                .addClass("layer-close")
                .appendTo($layer.children())
                .click(function () {
                    $layer.close();
                });

            $layer.open();
        });
}

$.fn.slideOutLeft = function () {
    var $target = this, $layer = $("<div/>");

    $layer.addClass("layer").css({
        backgroundColor: $target.css("background-color")
    });

    $("<div/>").addClass("layer-body").appendTo($layer);

    $layer.resize = function () {
        $layer.offset($target.offset()).css({
            height: $target.outerHeight(true) + "px",
            width: $target.outerWidth(true) + "px"
        });

        return $layer;
    }

    $layer.open = function () {
        return $layer.resize().removeClass("layer-out").addClass("layer-in");
    }

    $layer.close = function () {
        return $layer.removeClass("layer-in").addClass("layer-out");
    }

    return $layer
        .appendTo(document.body)
        .children()
        .load(url, params)
        .done(function () {
            $("<div><a>&times;</a></div>")
                .addClass("layer-close")
                .appendTo($layer.children())
                .click(function () {
                    $layer.close();
                });

            $layer.open();
        });
}