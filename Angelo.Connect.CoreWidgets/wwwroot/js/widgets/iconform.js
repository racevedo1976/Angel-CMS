define([], function () {

    return {
        attach: function ($form) {
           
            var $icons = $form.find(".icon-grid").children();
            var $preview = $form.find(".icon-preview");
            var $noResults = $form.find(".no-results");

            $form.find("input[name=Search]").on("keyup", function () {
                var phrase = $(this).val().trim().replace(/'/g, "").toLowerCase();

                if (phrase.length === 0) {
                    $icons.removeClass("filter-out");
                    $noResults.hide();
                    return;
                }

                // else
                var $results = $icons.filter("[title*='" + phrase + "']");

                $results.removeClass("filter-out");
                $icons.not($results).addClass("filter-out");

                $results.length == 0
                    ? $noResults.show()
                    : $noResults.hide();

            });

            $form.find("select[name=Size]").on("change", updatePreview);
            $form.find("input[name=Text]").on("change", updatePreview);
            $form.find("input[name=Tooltip]").on("change", updatePreview);

            $icons.on("click", function () {
                var result = $(this).data();

                $(this).addClass("active").siblings().removeClass("active");

                $form.find("input[name=Name]").val(result.iconName);
                updatePreview();
            });

            function updatePreview()
            {
                var css = $icons.filter(".active").data().iconCss,
                    size = $form.find("[name=Size]").val(),
                    text = $form.find("[name=Text]").val().trim(),
                    tooltip = $form.find("[name=Tooltip]").val().trim()

                $preview.css({ fontSize: size });
                $preview.attr("title", tooltip);
                $preview.find("i").attr("class", css);
                $preview.find("span").text(text);
            }
        }
    }
});