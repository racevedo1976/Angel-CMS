define(["console/tools", "resources/blog"], function (tools, resx) {


    return {
        attach: function ($this) {


            var $toolbar = $(".csc-content-toolbar");
            var categoryId = $("#blogCategoryForm").find("input[name=Id]").val();

            $toolbar.find("a[name=create]").on("click", function () {
                tools.navigate("/sys/console/blog/categories/create");
            });

            $toolbar.find("a[name=save],a[name=create]").on("click", function () {
                $("#blogCategoryForm").form().post().done(function (model) {
                    tools.refreshNavSection("blog");

                    // if creating, load the edit view after initial create
                    if (!categoryId) {
                        tools.navigate("/sys/console/blog/categories/" + model.Id);
                    }
                });
            });

            $toolbar.find("a[name=delete]").on("click", function () {
                tools.confirm(resx.blogCategoryConfirmDelete).done(function () {
                    $.ajax({ url: "/sys/api/blog/categories/" + categoryId, type: "DELETE" })
                    .done(function () {
                        tools.message(resx.blogCategoryDeleted, { icon: "fa fa-trash-o", sticky: true, hideContent: true });
                        tools.refreshNavSection("blog");
                    });
                })                
            });

            // loading related posts           
            if (categoryId) {
                $("#categoryPosts").load("/sys/console/blog/categories/" + categoryId + "/posts");
            }
        }
    }
});