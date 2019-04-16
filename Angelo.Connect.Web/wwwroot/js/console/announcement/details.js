define(["console/tools", "resources/announcement"], function (tools, resx) {
    return {
        attach: function ($this) {

            var $form = $this.find("#announcementPostSettings form");
            var $toolbar = $this.find(".csc-content-toolbar");

            var announcementPostId = $this.data().announcementPostId;

            var versionCode = $this.data().versionCode;
            var versionLabel = $form.find("input[name=VersionLabel]").val();
            var contentType = "AnnouncementPost";

            $toolbar.find("a[name=cancel]").on("click", function () {
                tools.navigate("/sys/console/announcement/posts");
            });

            $toolbar.find("a[name=settings]").on("click", function () {
                tools.navigate("/sys/console/announcement/posts/" + announcementPostId + "/settings");
            });

            $toolbar.find("a[name=design]").on("click", function () {
                tools.navigate("/sys/console/announcement/posts/" + announcementPostId + "/edit?version=" + versionCode);
            });
     
            $toolbar.find("a[name=version]").on("click", function () {
                tools.versionSelector(contentType, announcementPostId)
                    .done(function (versionCode, status) {
                        tools.navigate("/sys/console/announcement/posts/" + announcementPostId + "?version=" + versionCode);
                    });
            });
        }
    }
});