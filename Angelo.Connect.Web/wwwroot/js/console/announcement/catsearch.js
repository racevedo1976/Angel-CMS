define([], function () {
    return {
        attach: function ($this) {
            var searchTextTimerId,
                selectionMap = {};

            var $searchText = $this.find("input[name=searchText]"),
                $searchScope = $this.find("input[name=searchScope]");

            var categoryGrid = $this.find("#categorySearchGrid").getKendoGrid();

            // build initial selectionMap from any categories pass to us
            var modalData = $this.parents(".modal:first").data();

            if (modalData.selection) {
                modalData.selection.forEach(function (item) {
                    selectionMap[item.Id] = item;
                });
            }
            

            function rebindSearchGrid() {
                var scope = $searchScope.filter(":checked").val(),
                    text = $searchText.val();

                categoryGrid.dataSource.read({ text: text, shared: scope === "shared" });
                searchTextTimerId = undefined;
            }
         
            function updateModalResult() {
                var selectionArray = $.map(selectionMap, function (data, key) {
                    return data;
                });

                selectionArray = selectionArray.sort(function (a, b) {
                    return a.Title < b.Title ? -1 : 1;
                });

                $this.parents(".modal:first").find("a[name=apply]").data("selection", selectionArray);
            }

            function setGridSelection() {
                var dataItems = categoryGrid.dataItems(),
                    selectors = [];

                if (dataItems) {
                    dataItems.forEach(function (item) {
                        if (selectionMap[item.Id]) {
                            selectors.push("tr[data-uid='" + item.uid + "']");
                            selectionMap[item.Id] = item;
                        }
                    })
                }

                categoryGrid.clearSelection();

                if (selectors.length) 
                    categoryGrid.select(selectors.join());
            }

            $searchText.on("keypress", function () {
                if (searchTextTimerId) // throttling
                    window.clearTimeout(searchTextTimerId);

                searchTextTimerId = window.setTimeout(rebindSearchGrid, 500);
            });

            $searchScope.on("click", function () {
                $searchText.val("");
                rebindSearchGrid();
            });
           
            categoryGrid.bind("dataBound", setGridSelection);

            categoryGrid.bind("change", function () {
                var itemsInView = categoryGrid.dataItems();
                var $selectedRows = categoryGrid.select();

                if (itemsInView) {
                    itemsInView.forEach(function (item) {
                        if (selectionMap[item.Id])
                            delete selectionMap[item.Id];
                    });
                }

                $selectedRows.each(function () {
                    var item = categoryGrid.dataItem(this);
                    selectionMap[item.Id] = item;
                });           
         
                // update checkboxes
                $this.find(":checkbox[name=select]").prop("checked", false);
                $selectedRows.find(":checkbox[name=select]").prop("checked", true);

                // update modal result
                updateModalResult()
            });

            $this.on("change", ":checkbox[name=select]", function (event) {
                var $currentRow = $(this).closest("tr"),
                    item = categoryGrid.dataItem($currentRow);

                if (!this.checked && selectionMap[item.Id]) {
                    delete selectionMap[item.Id];
                }
                else {
                    selectionMap[item.Id] = item;
                }

                setGridSelection();
            });

        }
    }
});