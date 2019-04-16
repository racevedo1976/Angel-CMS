define(["console/tools", "resources/blog"], function (tools, resx) {
    return {
        attach: function ($this) {

            var $form = $this.find("#blogPostSettings form");
            var $toolbar = $this.find(".csc-content-toolbar");

            var blogPostId = $this.data().blogPostId;

            var versionCode = $this.data().versionCode;
            var versionLabel = $form.find("input[name=VersionLabel]").val();
            var contentType = "BlogPost";

            $toolbar.find("a[name=cancel]").on("click", function () {
                tools.navigate("/sys/console/blog/posts");
            });

            $toolbar.find("a[name=settings]").on("click", function () {
                tools.navigate("/sys/console/blog/posts/" + blogPostId + "/settings");
            });

            $toolbar.find("a[name=design]").on("click", function () {
                tools.navigate("/sys/console/blog/posts/" + blogPostId + "/edit?version=" + versionCode);
            });
     
            $toolbar.find("a[name=version]").on("click", function () {
                tools.versionSelector(contentType, blogPostId)
                    .done(function (versionCode, status) {
                        tools.navigate("/sys/console/blog/posts/" + blogPostId + "?version=" + versionCode);
                    });
            });
        }
    }
});