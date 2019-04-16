define(["console/tools", "resources/news"], function (tools, resx) {
    return {
        attach: function ($this) {

            var newsPostId = $this.data().newsPostId;
            var $categoryList = $this.find("ul[name=newsPostCategoryList]")

            function removeCategoryFromList() {
                $(this).parent().remove();
                toggleCategoryAlert();
            }

            function addCategoryToList(category) {
                if (getCategoryIds().indexOf(category.Id) >= 0)
                    return; // don't add duplicates

                var $li = $("<li></li>").addClass("list-group-item"),
                    $x = $("<a></a>").addClass("pull-right text-danger"),
                    $icon = $("<i></i>").addClass("fa fa-times");

                $x.append($icon).on("click", removeCategoryFromList);
                $li.data({ categoryId: category.Id }).text(category.Title).append($x);
                $categoryList.append($li);
                toggleCategoryAlert();
            }

            function getCategoryIds() {
                return $categoryList.find("li").map(function () {
                    return $(this).data().categoryId;
                }).get().join();
            }

            function getCategories() {
                return $categoryList.find("li").map(function () {                   
                    return {
                        Id: $(this).data().categoryId,
                        Title: $(this).text()
                    }
                }).get();
            }

            function toggleCategoryAlert() {
                var $alert = $this.find("div[name=newsPostCategoryAlert]");

                getCategoryIds().length >  0
                    ? $alert.addClass("hidden") 
                    : $alert.removeClass("hidden");               
            }

            $this.find("a[name=save]").on("click", function () {
                var model = {
                    IsPrivate: $("input[name=IsPrivate]").is(":checked"),
                    CategoryIds: getCategoryIds()
                };

                $.post("/sys/api/news/posts/" + newsPostId + "/settings", model).done(function () {
                    tools.message(resx.saved);
                });
            });

            $this.find("a[name=categoryMap]").on("click", function () {
                var selection = getCategories();

                tools.dialog("/sys/console/news/categories/select")
                    .data({ selection: selection })
                    .close(function (btn) {
                        var selection = $(btn).data().selection;

                        $categoryList.empty();
                        if (selection) {
                            selection.forEach(addCategoryToList);
                        }
                        toggleCategoryAlert();
                    });
            });

            $this.find("a[name=categoryAdd]").on("click", function () {
                tools.prompt(resx.newsCategoryCreatePrompt, { required: true }).done(function (value) {
                    $.post("/sys/api/news/categories", { Title: value }).done(function (category) {
                        tools.refreshNavSection("news");
                        addCategoryToList(category);
                    });
                });
            });

            $this.find("a[name=categoryUnmap]").on("click", removeCategoryFromList);

            
        }
    }
});