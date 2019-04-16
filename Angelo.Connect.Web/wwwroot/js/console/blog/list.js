define(["console/tools", "resources/blog"], function (tools, resx) {
    return {
        attach: function ($self) {
            var contentType = "BlogPost";

            function navigateToEdit(id, version) {
                tools.navigate("/sys/console/blog/posts/" + id + "/edit?version=" + version);
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
                var blogPostId = $(this).data().id;

                // confirm, refresh, then show message
                tools.confirm(resx.blogPostConfirmDelete, { notes: resx.cantUndo }).done(function () {
                    $.ajax({
                        url: "/sys/api/blog/posts/" + blogPostId,
                        type: "DELETE"
                    })
                    .done(function () {
                        tools.navigate("/sys/console/blog/posts").done(function () {
                            tools.message(resx.blogPostDeleted, { icon: "fa fa-trash-o" });
                        });
                    });
                });
            });


        }
    }
});