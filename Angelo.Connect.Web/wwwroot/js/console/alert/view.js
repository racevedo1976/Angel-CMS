define(["console/tools", "resources/alert"], function (tools, resx) {
    return {
        attach: function ($this) {

            var $form = $this.find("#blogPostSettings form");
            var $toolbar = $this.find(".csc-content-toolbar");

            var alertId = $this.data().id;

            var versionCode = $this.data().versionCode;
            var versionLabel = $form.find("input[name=VersionLabel]").val();
            var contentType = "SiteAlert";

            $toolbar.find("a[name=cancel]").on("click", function () {
                tools.navigate("/sys/console/sitealerts/List");
            });

            $toolbar.find("a[name=design]").on("click", function () {
                tools.navigate("/sys/console/sitealerts/edit/" + alertId + "?versionCode=" + versionCode);

            });
     
            $toolbar.find("a[name=version]").on("click", function () {
                tools.versionSelector(contentType, alertId)
                    .done(function (versionCode, status) {
                        tools.navigate("/sys/console/sitealerts/view/" + alertId + "?version=" + versionCode);
                    });
            });
        }
    }
});