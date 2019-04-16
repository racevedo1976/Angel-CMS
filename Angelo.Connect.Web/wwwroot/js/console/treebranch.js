define([], function () {
    var module = {};

    module.attach = function ($ul, data) {
        var treeType = $ul.parents(".csc-nav-root:first").data().treeType;

        $ul.children().each(function () {
            var $li = $(this), id = $li.attr("id"),
                $expander = $li.children(":first").find("a.csc-nav-expander"),
                $icon = $expander.children(":first");


            $expander.on("click", function () {
                var $branch = $li.children(".csc-nav-branch");
                var nodeType = $li.data().nodeType;

                if (!$branch.data().branchLoaded) {
                    $branch
                        .load("/sys/console/nav/" + treeType + "/branch/" + id, { type: nodeType })
                        .done(function () {
                            $branch.hide().slideDown(100);
                            $branch.data().branchLoaded = true;
                            $icon.removeClass("fa-plus-square").addClass("fa-minus-square");
                        }) 
                }
                else {                
                    $icon.removeClass("fa-plus-square fa-minus-square").addClass(
                        $branch.is(":visible") ? "fa-plus-square" : "fa-minus-square"
                    );

                    $branch.slideToggle(100);
                }

            })
        });
    };

    return module;
});