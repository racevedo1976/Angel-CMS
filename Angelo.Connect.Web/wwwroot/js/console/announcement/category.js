define(["console/tools", "resources/announcement"], function (tools, resx) {


    return {
        attach: function ($this) {


            var $toolbar = $(".csc-content-toolbar");
            var categoryId = $("#announcementCategoryForm").find("input[name=Id]").val();

            $toolbar.find("a[name=create]").on("click", function () {
                tools.navigate("/sys/console/announcement/categories/create");
            });

            $toolbar.find("a[name=save],a[name=create]").on("click", function () {
                $("#announcementCategoryForm").form().post().done(function (model) {
                    tools.refreshNavSection("announcement");

                    // if creating, load the edit view after initial create
                    if (!categoryId) {
                        tools.navigate("/sys/console/announcement/categories/" + model.Id);
                    }
                });
            });

            $toolbar.find("a[name=delete]").on("click", function () {
                tools.confirm(resx.announcementCategoryConfirmDelete).done(function () {
                    $.ajax({ url: "/sys/api/announcement/categories/" + categoryId, type: "DELETE" })
                    .done(function () {
                        tools.message(resx.announcementCategoryDeleted, { icon: "fa fa-trash-o", sticky: true, hideContent: true });
                        tools.refreshNavSection("announcement");
                    });
                })                
            });

            // loading related posts           
            if (categoryId) {
                $("#categoryPosts").load("/sys/console/announcement/categories/" + categoryId + "/posts");
            }
        }
    }
});