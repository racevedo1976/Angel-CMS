define(["resources/common"], function (resx) {

    return {
        attach: function ($widget) {
            
            var userId = $widget.data().userId;
            var driveUrl = $widget.data().driveUrl;
            var $triggerType = $widget.find("select[name=TriggerType]");

            function toggleTriggerType(value) {
                var group = $triggerType.children(":selected").val();

                $widget.find(".form-group").not(":first")
                    .hide()
                    .filter(function () {
                        return $(this).data().group.split(",").indexOf(group) >= 0;
                    })
                    .show();
            }

            $widget.find("a[name=selectImage]").on("click", function () {
                var $input = $widget.find("input[name=ImageSrc]");

                $.mediaBrowser({ title: resx.selectImage, userId: userId, driveUrl: driveUrl }).done(function (image) {
                    if (image) {
                        var imageUrl = image.isCropped ? image.croppedUrl : image.url;

                        $input.val(imageUrl);
                    }
                })
            });

            $widget.find("a[name=cropImage]").on("click", function () {
                var $input = $widget.find("input[name=ImageSrc]");

                $.imageCropper({
                    editImageMode: true,           //set to true if image already cropped (previously) and we are just edting that cropped version
                    editImageUrl: $input.val(),    //if editImageMode = true, then pass the URL used for this cropped image with coordinates.
                    driveUrl: driveUrl             // *Require*  drive host, url.
                })
                .done(function (image) {
                    if (image) {
                        $input.val(image.croppedUrl);
                    }
                });
            });



            $triggerType.on("change", toggleTriggerType);

            toggleTriggerType();
        }
    }
});