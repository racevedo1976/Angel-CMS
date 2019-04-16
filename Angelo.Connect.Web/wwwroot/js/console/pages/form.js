define(["console/tools"], function (tools) {
    return {
        attach: function ($self) {
            
            var $toolbar = $(".csc-content-toolbar");
            var $form = $self.find("form");

            $toolbar.on("click", "a[name=save]", function () {
                tools.message("Saving ...");
                $("#ucPageForm").form().post().done(function () {
                    tools.message("Page Updated");
                    tools.refreshNavSection("pages");
                });
            });

            $toolbar.on("click", "a[name=create]", function () {
                tools.message("Creating ...");

                $("#ucPageForm").find("input[name=Id]").val(undefined);

                $("#ucPageForm").form().post().done(function (page) {
                    tools.message("Page Created")

                    tools.refreshNavSection("pages");
                    tools.navigate("/sys/console/pages/" + page.Id);
                });
            });

            $toolbar.on("click", "a[name=delete]", function () {
                var message1 = "Are you sure you want to remove this page?";
                var message2 = "This page has been deleted";

                tools.confirm(message1).done(function () {
                    var pageId = $form.find("input[name=Id]").val();

                    $.ajax({ url: "/sys/console/pages/" + pageId, type: "DELETE" })
                        .fail(function(error){
                            $form.form().setErrors(error.responseText);
                        })
                        .done(function () {
                            $form.form().clearErrors();
                            tools.message(message2, { icon: "fa fa-trash-o", sticky: true, hideContent: true });
                            tools.refreshNavSection("blog");
                        });
                })
            });

            $form.on("change", ":checkbox[name=IsPrivate]", function() {
                var value = $(this).is(":checked") ? "block" : "none";

                $("#pagePrivateSecurity").css({ display: value });
            });
        }
    }
});