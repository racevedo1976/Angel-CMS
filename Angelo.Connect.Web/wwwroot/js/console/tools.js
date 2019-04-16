define(["resources/common"], function (resx) {
    
    $(document).on("click", ".csc-nav-panel a[data-route]", function () {
        $('.csc-content-message').hide();
    });

    var tools = {};

    tools.refreshNavSection = function (component) {
        return $(".csc-nav-section")
            .filterByData({ componentType: component })
            .load("/sys/console/nav/" + component);
    }

    tools.navigate = function (route) {
        $('.csc-content-message').hide();

        return $(".csc-content-host").load(route);
    }
  
    tools.dialog = function () {
        return $.dialog.apply(window, arguments);
    }

    tools.versionSelector = function (contentType, contentId) {
        var url = "/sys/console/versions/" + contentType + "/" + contentId;
        var task = $.Deferred();

        $.dialog(url)
            .close(function (button) {
                var data = $(button).data();

                if (data.versionCode) {
                    task.resolve(data.versionCode, data.versionStatus);
                }

                task.reject();
            })

        return task.promise();
    }

    tools.message = function (message, options) {
        var defaults = { delay: 1500, sticky: false, type: "default", icon: "fa fa-info", hideContent: false },
            task = $.Deferred();

        options = $.extend({}, defaults, options);

        if (message instanceof $)
            message = message.html();

        var $content = $(".csc-content-host").children();
        var $message = $("<div></div>").addClass("alert alert-" + options.type).html(message);

        if (options.icon) {
            $message.prepend(" &nbsp;")
            $message.prepend($("<i></i>").addClass(options.icon));
        }

        if (options.hideContent)
            $content.hide();

        $('.csc-content-message').empty().append($message).fadeIn(200);
        
        if (!options.sticky) {            
            window.setTimeout(function() {
                $('.csc-content-message').hide();

                if (options.hideContent)
                    $content.show();

                task.resolve($message);
            }, options.delay);

            return task.promise();
        }

        return $message;
    }

    tools.prompt = function (message, options) {
        var defaults = { icon: "", title: "", value: "", placeholder: "", notes: "", required: false },
            $popup = $($("#consolePopupTemplate").html()),
            task = $.Deferred();

        options = $.extend({}, defaults, options);

        $popup.find(".csc-confirm-buttons").hide();
        $popup.find(".csc-popup-icon").attr("class", options.icon);
        $popup.find(".csc-popup-title").html(options.title);
        $popup.find(".csc-popup-notes").html(options.notes);
        $popup.find(".csc-popup-message").html(message || resx.promptMessage);

        $popup.find(".csc-popup-input")
            .attr("placeholder", options.placeholder)
            .val(options.value);

        $popup.appendTo(document.body).find(".modal-button").on("click", function () {
            var value = $popup.find(".csc-popup-input").val().trim();
            $popup.remove();

            // TODO: Handled required values
            if($(this).attr("name") === "cancel" || (options.required && value === ""))
                return task.fail();
            
            task.resolve(value);
        })

        $popup.find(".csc-popup-input").focus();

        return task.promise();
    }

    tools.confirm = function (message, options) {
        var defaults = { icon: "", title: "", notes: "" },
            $popup = $($("#consolePopupTemplate").html()),
            task = $.Deferred()

        options = $.extend({}, defaults, options);

        $popup.find(".csc-prompt-buttons").hide();
        $popup.find(".csc-popup-input").hide();
        $popup.find(".csc-popup-icon").attr("class", options.icon);
        $popup.find(".csc-popup-title").html(options.title);
        $popup.find(".csc-popup-notes").html(options.notes);
        $popup.find(".csc-popup-message").html(message || resx.confirmMessage);

        $popup.appendTo(document.body).find(".modal-button").on("click", function () {
            $popup.remove();
            
            if ($(this).attr("name") === "cancel")
                return task.fail();

            task.resolve();
        })

        return task.promise();
    }

    return tools;
});