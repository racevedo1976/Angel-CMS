$(function () {

    var attachSelectList = function (container) {
        $(container).find("[data-selectable]").each(function () {
            var $list = $(this),
                listId = this.id,
                data = $list.data();

            var $items = data.selectable
			    ? $list.find(data.selectable)
			    : $list.children();

            function select(id) {            
                var $selected = $list.find("#" + id);

                $items.removeClass("active");
                $selected.addClass("active")
                $list.data({ selected: id });

                $.trigger.call($list, listId + ".select", {
                    item: $selected[0],
                    id: id
                });
            }

            $items.on("click", function (event) {
                select(this.id);
            });

            if (data.selected) {
                select(data.selected);
            }
        });
    }

    $.on("dom.change", function () {
        attachSelectList(this);
    });

    attachSelectList(document);
});
	