define([], function () {
    return {
        attach: function ($container) {

            var $categoryToggle = $container.find("[name='category-toggle']");
            var $categoryList = $container.find("[name='category-list']");

            $categoryToggle.on("click", function (event) {
               
                var $icon = $categoryToggle.find("i.fa");

                $icon.removeClass("fa-plus-square fa-minus-square");

                if ($categoryList.is(":hidden")) {
                    $categoryList.slideDown(200);
                    $icon.addClass("fa-minus-square");
                }
                else {
                    $categoryList.slideUp(200);
                    $icon.addClass("fa-plus-square");
                }
            });

        }
    }
});