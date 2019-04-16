define(["console/tools", "resources/announcement"], function (tools, resx) {
    return {
        attach: function ($self) {

            var $form = $self.find("#announcementPostSummary form");
            var $toolbar = $self.find(".csc-content-toolbar");

            var userId = $self.data().userId;
            var driveUrl = $self.data().driveUrl;
            var categoryCount = $self.data().categoryCount;

            var announcementPostId = $form.find("input[name=Id]").val();
            var versionCode = $form.find("input[name=VersionCode]").val();
            var versionLabel = $form.find("input[name=VersionLabel]").val();
           
            var needsVersionLabel = !versionLabel;
            var contentType = "AnnouncementPost";

            $toolbar.find("a[name=cancel]").on("click", function () {
                tools.navigate("/sys/console/announcement/posts");
            });

            $toolbar.find("a[name=save],a[name=publish]").on("click", function () {

                var publish = $(this).attr("name") === "publish";
                var categoryIds = getCategoryIds();

                function performUpdate() {
                    $form.find("input[name=ShouldPublish]").val(publish);
                    $form.find("input[name=CategoryIds]").val(categoryIds);

                    $form.form().post()
                        .done(function (model) {
                            var message = publish ? resx.published : resx.saved;

                            tools.navigate("/sys/console/announcement/posts/" + model.Id)
                                .done(function () {
                                    // show publish message after navigation
                                    tools.message(message, { icon: "fa fa-check" });
                                });
                        })
                        .fail(function () {
                            // form submission error - focus to main form
                            $(".csc-content-tabs .nav-tabs").tabs().show(0);
                        });
                }

                if (needsVersionLabel) {
                    tools.prompt(resx.versionLabelPrompt, {notes: resx.versionLabelExample, required: false})
                        .done(function (name) {
                            $form.find("input[name=NewVersionLabel]").val(name);
                            needsVersionLabel = false;
                        })
                        .always(performUpdate);
                }
                else {
                    performUpdate();
                }             
            });

            $toolbar.find("a[name=delete]").on("click", function () {              
                tools.confirm(resx.announcementPostConfirmDelete, { notes: resx.cantUndo }).done(function () {
                    $.ajax({
                        url: "/sys/api/announcement/posts/" + announcementPostId + "?version=" + versionCode,
                        type: "DELETE"
                    })
                    .done(function () {
                        tools.message(resx.announcementPostDeleted, { icon: "fa fa-trash-o", sticky: true, hideContent: true });
                    });
                });
            });

            $toolbar.find("a[name=version]").on("click", function () {
                tools.versionSelector(contentType, announcementPostId)
                    .done(function (versionCode, status) {
                        tools.navigate("/sys/console/announcement/posts/" + announcementPostId + "/edit?version=" + versionCode)
                    });
            });

            $self.find("a[name=selectImage]").on("click", function () {
                var $input = $self.find("input[name=Image]"),
                    $preview = $self.find("img[name=imagePreview]");

                $.mediaBrowser({title: resx.selectImage, userId: userId, driveUrl: driveUrl }).done(function (image) {
                    if (image) {
                        var imageUrl = image.isCropped ? image.croppedUrl : image.url;

                        $input.val(imageUrl);
                        $preview.attr('src', imageUrl);
                    }
                })
            });

            $self.find("a[name=cropImage]").on("click", function () {
                var $input = $self.find("input[name=Image]");

                $.imageCropper({
                    editImageMode: true,           //set to true if image already cropped (previously) and we are just edting that cropped version
                    editImageUrl: $input.val(),    //if editImageMode = true, then pass the URL used for this cropped image with coordinates.
                    driveUrl: driveUrl             // *Require*  drive host, url.
                })
                .done(function (image) {
                    if (image) {
                        $input.val(image.croppedUrl);
                        $('#postImage').attr('src', image.croppedUrl);
                    }
                });
            });

            // duplicated from settings.js
            // consider moving to shared module

            var $categoryList = $self.find("ul[name=announcementPostCategoryList]")
         
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
                var $alert = $self.find("div[name=announcementPostCategoryAlert]");

                getCategoryIds().length > 0
                    ? $alert.addClass("hidden")
                    : $alert.removeClass("hidden");
            }

            $self.find("a[name=categoryMap]").on("click", function () {
                var selection = getCategories();

                tools.dialog("/sys/console/announcement/categories/select")
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

            $self.find("a[name=categoryAdd]").on("click", function () {
                tools.prompt(resx.announcementCategoryCreatePrompt, { required: true }).done(function (value) {
                    $.post("/sys/api/announcement/categories", { Title: value }).done(function (category) {
                        tools.refreshNavSection("announcement");
                        addCategoryToList(category);
                    });
                });
            });

            $self.find("a[name=categoryUnmap]").on("click", removeCategoryFromList);

        }
    }
});