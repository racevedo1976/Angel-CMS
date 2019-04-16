define(["console/tools"], function (consoleTools) {
    
    // Returning a function will cause the function to be evaulated before calling the resulting object.attach() method
    // This is useful for factory situations where you'll want a new instance of the module, such as when internally
    // tracking the state of multiple dom objects indenpendently

    // Returning a plain object means this same object will have it's attach method invoked for each dom element
    // it's associated with. Useful for simple event attaching - return a function if you need a new instance of
    // the module for each dom element

    // since only one dialog will ever be open on a page, we just need a single object instance for the console

    return {
        attach: function ($this) {
            var defaultRoute = $this.find(".csc-content-host").data().defaultRoute;

            $(document).on("click", "a.csc-modal-close", function () {
                $(this).parents(".modal:first").remove();
            });

            $(document).on("click", "a.csc-nav-panel-toggle", function () {
                var $panel = $this.find(".csc-nav-panel");

                !$panel.is(":visible")
                    ? $panel.addClass("csc-nav-panel-visible")
                    : $panel.removeClass("csc-nav-panel-visible")
            });

            $(document).on("click", ".csc-nav-panel a[data-route]", function () {
                $this.find(".csc-nav-panel").removeClass("csc-nav-panel-visible");
            });

            $(document).on("click", ".csc-content-panel .nav-tabs a", function () {
                var $tab = $($(this).attr("href"));

                if ($tab.length) {
                    $(".csc-content-panel").find("div[data-tab]").each(function () {
                        var $div = $(this), $target = $($div.data().tab);

                        $div.hide();
                        if ($target.length && $target[0] == $tab[0])
                            $div.show();
                    })
                }

            })

            //console.navigate(defaultRoute);
        }       
    };
});