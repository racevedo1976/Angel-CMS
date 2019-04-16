define(["console/tools", "resources/news"], function (tools, resx) {


    return {
        attach: function ($this) {


            var $toolbar = $(".csc-content-toolbar");
            var categoryId = $("#newsCategoryForm").find("input[name=Id]").val();

            $toolbar.find("a[name=create]").on("click", function () {
                tools.navigate("/sys/console/news/categories/create");
            });

            $toolbar.find("a[name=save],a[name=create]").on("click", function () {
                $("#newsCategoryForm").form().post().done(function (model) {
                    tools.refreshNavSection("news");

                    // if creating, load the edit view after initial create
                    if (!categoryId) {
                        tools.navigate("/sys/console/news/categories/" + model.Id);
                    }
                });
            });

            $toolbar.find("a[name=delete]").on("click", function () {
                tools.confirm(resx.newsCategoryConfirmDelete).done(function () {
                    $.ajax({ url: "/sys/api/news/categories/" + categoryId, type: "DELETE" })
                    .done(function () {
                        tools.message(resx.newsCategoryDeleted, { icon: "fa fa-trash-o", sticky: true, hideContent: true });
                        tools.refreshNavSection("news");
                    });
                })                
            });

            // loading related posts           
            if (categoryId) {
                $("#categoryPosts").load("/sys/console/news/categories/" + categoryId + "/posts");
            }
        }
    }
});