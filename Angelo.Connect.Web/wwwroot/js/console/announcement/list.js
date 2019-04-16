define(["console/tools", "resources/announcement"], function (tools, resx) {
    return {
        attach: function ($self) {
            var contentType = "AnnouncementPost";

            function navigateToEdit(id, version) {
                tools.navigate("/sys/console/announcement/posts/" + id + "/edit?version=" + version);
            }

            $self.on("click", "a[name=design]", function () {
                var data = $(this).data();

                // if the primary version is already a draft version, go directly to it
                if (data.status <= 1) {
                    return navigateToEdit(data.id, data.version);
                }

                // else show the version selector
                tools.versionSelector(contentType, data.id)
                   .done(function (versionCode, status) {
                       navigateToEdit(data.id, versionCode);
                   });
            })
           
            $self.on("click", "a[name=delete]", function () {
                var announcementPostId = $(this).data().id;

                // confirm, refresh, then show message
                tools.confirm(resx.announcementPostConfirmDelete, { notes: resx.cantUndo }).done(function () {
                    $.ajax({
                        url: "/sys/api/announcement/posts/" + announcementPostId,
                        type: "DELETE"
                    })
                    .done(function () {
                        tools.navigate("/sys/console/announcement/posts").done(function () {
                            tools.message(resx.announcementPostDeleted, { icon: "fa fa-trash-o" });
                        });
                    });
                });
            });


        }
    }
});