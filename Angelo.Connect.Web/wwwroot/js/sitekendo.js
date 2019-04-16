$(document).ready(function () {

    function attachKendoEvents(container) {
        $(container).find(".k-widget,.k-input").not(".k-events-attached").each(function () {
            var kendoEvents = {
                //editors
                kendoAutoComplete: ["select", "change", "open", "close", "filtering", "dataBound"],
                kendoColorPicker: ["select", "change", "open", "close"],
                kendoComboBox: ["select", "change", "open", "close", "filtering", "dataBound"],
                kendoDatePicker: ["change", "open", "close"],
                kendoDateTimePicker: ["change", "open", "close"],
                kendoDropDownList: ["select", "change", "open", "close", "filtering", "dataBound"],
                kendoEditor: ["select", "change", "execute", "paste"],
                kendoMaskedTextBox: ["change"],
                kendoMultiSelect: ["select", "deselect", "change", "open", "close", "filtering", "dataBound"],
                kendoNumericTextBox: ["change", "spin"],
                kendoRangeSlider: ["change", "slide"],
                kendoTimePicker: ["change", "open", "close"],
                kendoUpload: ["select", "cancel", "complete", "progress", "remove", "error", "success", "upload"],

                //navigation
                kendoButton: ["click"],
                kendoContextMenu: ["select", "open", "close", "activate", "deactivate"],
                kendoPanelBar: ["select", "expand", "collapse", "activate", "contentLoad", "error"],
                kendoTabStrip: ["select", "activate", "show", "contentLoad", "error"],
                kendoToolBar: ["click", "toggle", "open", "close", "overflowOpen", "overflowClose"],
                kendoTreeView: ["select", "check", "change", "collapse", "expand", "dragstart", "dragend", "drag", "drop", "navigate"],

                //data management
                kendoGrid: ["change", "edit", "save", "remove", "dataBound", "dataBinding", "sort", "filter", "group", "page"],
                kendoListView: ["change", "edit", "save", "remove", "dataBound", "dataBinding", "sort", "filter", "group", "page", "dragstart", "dragend", "drag", "drop"],
                kendoTreeList: ["change", "edit", "save", "remove", "dataBound", "dataBinding", "sort", "filter", "dragstart", "dragend", "drag", "drop"],

                //layout
                kendoDialog: ["initOpen", "open", "close", "show", "hide"],
                kendoNotification: ["show", "hide"],
                kendoSplitter: ["expand", "collapse", "contentLoad", "resize"],
                kendoTooltip: ["show", "hide"],
                kendoWindow: ["open", "close", "activate", "deactivate", "resize", "dragstart", "dragend"],

                //media
                kendoMediaPlayer: ["play", "pause", "end", "ready", "timeChange", "volumeChange"],
            }

            var widgetNode = this,
                widgetId = this.id || this.name,
                widgetData = $(this).data();

            for (var key in widgetData) {
                if (key.substr(0, 5) === "kendo") {
                    for (var i = 0; i < (kendoEvents[key] || []).length; i++) {
                        (function (widget, eventName) {
                            widget.bind(eventName, function (data) {
                                return $.trigger.call(widgetNode, widgetId + "." + eventName, data);
                            });
                        })(widgetData[key], kendoEvents[key][i]);
                    }
                }
            }

            $(this).addClass("k-events-attached");
        })

    }

    $.on("dom.change", function (event, data) {
        window.setTimeout(function () {
            attachKendoEvents(data.container);
        }, 500);
    });

    attachKendoEvents(document);

    // This function is used by the CustomEvent extension method of the Kendo grid column class.
    $(document).on("click", "[data-kendo-event-cid][data-kendo-event-name]", function () {
        var $data = $(this).data();
        var cid = $data.kendoEventCid,
            eventName = $data.kendoEventName;
        var targetTag = $("#" + cid + "_component"),
            targetEvent = cid + "." + eventName;
        var eventData = Object;
        for (var item in $data) {
            if (item.indexOf("kendoParam") == 0) {
                eventData[item.substring(10)] = $data[item];
            }
        }
        $.trigger.call(targetTag[0], targetEvent, eventData);
    });




});


