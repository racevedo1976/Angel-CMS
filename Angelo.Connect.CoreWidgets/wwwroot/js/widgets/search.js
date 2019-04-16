define([], function () {
    return {
        attach: function ($div) {

            var $input = $div.find("input");

            function _submitSearch() {
                var q = window.encodeURIComponent($input.val());
                location.href = "/sys/search?q=" + q;
            };

            $input.on("keypress", function (event) {
                if (event.which == 13 || event.keyCode == 13) {
                    _submitSearch();
                }
            });

            $input.next().on("click", _submitSearch);
        }
    }
});