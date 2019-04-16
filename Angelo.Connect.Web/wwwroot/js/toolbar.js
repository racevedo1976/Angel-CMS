$.fn.toolbar = function () {
    return $(this).data().toolbar;
}

$(function () {
    function createToolbarComponents(container) {
        $(container).find(".toolbar").not(".events-attached").each(function () {
            var $toolbar = $(this),
                toolbarId = this.id || this.name,
                targetCid = $(this).data().targetCid || toolbarId;

            var toolbarObject = {
                id: toolbarId,
                add: function (options) {
                    if(typeof options !== "object")
                        throw new Error("Toolbar options: { id: <string>, text: <string>, tooltip: <string>, icon: <string>, callback: <function>}")

                    var $button = $("<a></a>")
                        .attr({ name: options.name, title: options.tooltip, class: options.icon || "" })
                        .on("click", function (event) {
                            if (typeof options.click === "function" && !$button.is(":disabled"))
                                options.click.call(this, event);

                            triggerGlobalClickEvent($button)
                        });

                    if (options.text) {
                        $button.html("<span> " + options.text + "</span>");
                    }

                    $toolbar.append($button);
                },
                disable: function(name){
                    $toolbar.find("a[name='" + name + "']").attr("disabled", "disabled");
                },
                enable: function (name) {
                    $toolbar.find("a[name='" + name + "']").removeAttr("disabled");
                },
                show: function(name){
                    $toolbar.find("a[name='" + name + "']").removeClass('hidden'); //.show()
                },
                hide: function(name){
                    $toolbar.find("a[name='" + name + "']").addClass('hidden'); //.hide();
                },
                empty: function () {
                    $toolbar.empty();
                }
            }

            function triggerGlobalClickEvent($button) {
                if ($button.is(":disabled")) return;

                if ($button.length && $button[0].name) {
                    $.trigger.call($button[0], targetCid + "." + $button[0].name, {
                        button: $button[0].name,
                        toolbar: toolbarObject
                    });
                }
            }

            $toolbar
                .addClass("events-attached")
                .data({ toolbar: toolbarObject });

            $toolbar.children()
                .each(function () {
                    var $button = $(this);
                    if ($button.text().trim().length && !$button.find("span").length) {
                        $button.html("<span> " + $button.html() + "</span>");
                    }
                })
                .on("click", function () {
                    triggerGlobalClickEvent($(this));
                })

            if (toolbarId) {
                $.trigger.call($toolbar[0], toolbarId + ".load", toolbarObject);
            }
        });
    };

    // create only for the dom node that changed 
    $.on("dom.change", function (event, data) {
        createToolbarComponents(data.container);
    });

    // create for entire document on initial page load;
    createToolbarComponents(document);
});
