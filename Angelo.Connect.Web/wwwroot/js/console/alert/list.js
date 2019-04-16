define(["console/tools", "resources/alert"], function (tools, resx) {
    return {
        attach: function ($container) {
            var contentType = "SiteAlert";


            function navigateToEdit(id, version) {
                tools.navigate("/sys/console/sitealerts/edit/" + id + "?versionCode=" + version);
            }

            $container.on("click", "a[name=design]", function () {
                var data = $(this).data();

                // if the primary version is already a draft version, go directly to it
                if (data.status <= 1) {
                    return navigateToEdit(data.id, data.version);
                }

                // else show the version selector
                tools.versionSelector(contentType, data.id)
                    .done(function (versionCode, status) {
                        navigateToEdit(data.id, versionCode);
                    })
                    .fail(function() {
                        tools.navigate("/sys/console/sitealerts/list");
                    });
            })


        }
    }
});