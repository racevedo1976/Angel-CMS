define([], function () {

    return {
        attach: function ($widget) {
            
            var triggerType = $widget.data().triggerType;
            var timer = $widget.data().timer;
            var editable = $widget.data().editable;
            var widgetId = $widget.data().widgetId;
            var treeId = $widget.data().treeId;
            
            function showLightbox() {
                var $popup = $(document).find(".lightbox-popup-" + widgetId);

                if ($popup.length) {
                    $popup.show();
                    $popup.prev().show();
                }
                else {
                    createLightbox();
                }

                window.scrollTo(0, 0);
            }

            function createLightbox() {
                var $tree = $("<div></div>").addClass("content-tree cs-content-tree lightbox-popup");
                var $node = $widget.parents(".content-node:first").clone();
                var $popup = $($widget.find(".lightbox-template").html());
                var $background = $("<div></div>").addClass("lightbox-popup-bg");

                if (editable == "True") {
                    $tree.addClass("editable");
                    $tree.attr("id", treeId);
                }

                $node.removeAttr("style");
                $node.empty().append($popup);
                $tree.append($node);
                $tree.addClass("lightbox-popup-" + widgetId);

                $(document.body).append($background);
                $(document.body).append($tree);

                $node.find(".content-zone.editable").each(function () {
                    $(this).contentZone().init();
                });

                $node.find(".content-node.editable").each(function () {
                    $(this).contentNode().init();
                });

                $node.find(".lightbox-close").on("click", function () {
                    $tree.hide();
                    $tree.prev().hide();
                })
            }



            $widget.find(".lightbox-trigger").on("click", function () {
                showLightbox();
            });

            $.on("lightbox.open", function (event, id) {
                if(id == widgetId) showLightbox();
            });
                     
            if(editable == "False" && triggerType == "Timer")
            {
                // Note: Timer value is stored in seconds.
                //       Convert to Milliseconds
                window.setTimeout(showLightbox, timer * 1000);
            }

        }
    }
});