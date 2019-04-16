define([], function () {

    return {
        attach: function ($self) {

            var dialog = $self.parents(".modal:first").dialog();

            // wire up update button
            $self.find("button[name=save]").on("click", function () {
                $self.find("form").form().post().done(function () {
                    $("#sitecss").attr("href", "/sys/site/css?" + +new Date());

                    dialog.close();
                });
            });

        }
    }
});