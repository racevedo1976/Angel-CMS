define([], function () {

    return {
        attach: function ($tabs) {
            // look for first .tab-content container in the modal
            var $modal = $tabs.parents(".modal:first");
            var $tabContent = $modal.find(".tab-content:first");

            if ($tabContent.length === 0)
                return;

            // ensure first tab panel is marked active (using bs classes)
            $tabContent.children(".tab-pane:first").show().addClass("active");



            // wire up click events
            $tabs.children().each(function (idx) {

                // get tab panel at this same index
                var $panel = $tabContent.children(".tab-pane:eq(" + idx + ")");

                // handle click event 
                if ($panel.length) {

                    $(this).children("a").on("click", function () {
                        var $tab = $(this).parent();

                        $tab.siblings().removeClass("active");
                        $tab.addClass("active");

                        $panel.siblings().removeClass("active").hide();
                        $panel.show().addClass("active");
                    })
                }
            });

        }
    }
});