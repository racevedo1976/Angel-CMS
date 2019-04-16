define(["console/tools"], function (console) {
    return {
        attach: function () {

            var pagesTreeList = $("#userPagesTreeList").getKendoTreeList();

            pagesTreeList.bind("drop", function (data) {

                // if no parent, then exit - don't allow root pages
                if (!data.valid || !data.destination || !data.destination.Id) {
                    console.message("You cannot move that here.")
                    pagesTreeList.dataSource.read();
                    return;
                }

                $.post(
                    "/sys/console/pages/" + data.source.Id + "/move",
                    { parentId: data.destination.Id }
                )
                .done(function () {
                    pagesTreeList.dataSource.read();
                });
            });

        }
    }
});