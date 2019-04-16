define(["console/tools", "resources/news"], function (tools, resx) {
    return {
        attach: function ($this) {

            var $form = $this.find("#newsPostSettings form");
            var $toolbar = $this.find(".csc-content-toolbar");

            var newsPostId = $this.data().newsPostId;

            var versionCode = $this.data().versionCode;
            var versionLabel = $form.find("input[name=VersionLabel]").val();
            var contentType = "NewsPost";

            $toolbar.find("a[name=cancel]").on("click", function () {
                tools.navigate("/sys/console/news/posts");
            });

            $toolbar.find("a[name=settings]").on("click", function () {
                tools.navigate("/sys/console/news/posts/" + newsPostId + "/settings");
            });

            $toolbar.find("a[name=design]").on("click", function () {
                tools.navigate("/sys/console/news/posts/" + newsPostId + "/edit?version=" + versionCode);
            });
     
            $toolbar.find("a[name=version]").on("click", function () {
                tools.versionSelector(contentType, newsPostId)
                    .done(function (versionCode, status) {
                        tools.navigate("/sys/console/news/posts/" + newsPostId + "?version=" + versionCode);
                    });
            });
        }
    }
});