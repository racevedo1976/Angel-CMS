define(["console/tools", "resources/alert"], function (tools, resx) {
    return {
        attach: function ($this) {


            function startDateChange() {
                checkDateRange('startdate_updated');
            }

            function endDateChange() {
                checkDateRange('enddate_updated');
            }

            function checkDateRange(whoUpdated) {
                
                var startDate = $("#alertStartDate").data("kendoDatePicker");
                var endDate = $("#alertEndDate").data("kendoDatePicker");

                //check if end date is less than this new start date, if it is then update the end date as well.
                if (new Date(kendo.toString(startDate.value(), 'd')) >
                    new Date(kendo.toString(endDate.value(), 'd'))) {

                    if (whoUpdated === 'startdate_updated') {
                        endDate.value(startDate.value());
                    }else
                        startDate.value(endDate.value());
                   
                    return;
                }
            }

            //kendo date init
            $this.find("#alertStartDate").kendoDatePicker({
                change: startDateChange
            });
            $this.find("#alertEndDate").kendoDatePicker({
                change: endDateChange
            });

            var $form = $this.find("#alertSettings form");
            var $toolbar = $this.find(".csc-content-toolbar");

            var alertId = $form.find("input[name=Id]").val();
            var versionCode = $form.find("input[name=VersionCode]").val();
            var versionLabel = $form.find("input[name=VersionLabel]").val();
           
            var needsVersionLabel = !versionLabel;
            var contentType = "SiteAlert";

            $toolbar.find("a[name=cancel]").on("click", function () {
                tools.navigate("/sys/console/sitealerts/List");
            });

            $toolbar.find("a[name=save],a[name=publish]").on("click", function () {

                var publish = $(this).attr("name") === "publish";

                function performUpdate() {
                    $form.find("input[name=ShouldPublish]").val(publish);

                    $form.form().post().done(function (model) {
                        if (!publish)
                            return tools.message(resx.saved, { icon: "fa fa-check" });

                        tools.navigate("/sys/console/sitealerts/view/" + model.Id + "?version=" + model.VersionCode)
                            .done(function () {
                                tools.message(resx.published, { icon: "fa fa-check" });
                            });

                    });
                }

                if (needsVersionLabel) {
                    tools.prompt(resx.versionLabelPrompt, {notes: resx.versionLabelExample, required: false})
                        .done(function (name) {
                            $form.find("input[name=NewVersionLabel]").val(name);
                            needsVersionLabel = false;
                        })
                        .always(performUpdate);
                }
                else {
                    performUpdate();
                }             
            });

            $toolbar.find("a[name=delete]").on("click", function () {
                tools.confirm(resx.alertConfirmDelete, {notes: resx.cantUndo}).done(function (answer) {
                    
                    tools.navigate("/sys/console/sitealerts/delete/" + alertId);
                    $.ajax({
                        url: "/sys/console/sitealerts/delete/" + alertId + "?version=" + versionCode,
                            type: "DELETE"
                        })
                        .done(function () {
                            tools.message(resx.alertDeleted, { icon: "fa fa-trash-o", sticky: true, hideContent: true });
                        });

                });

               
            });

            $toolbar.find("a[name=version]").on("click", function () {
                tools.versionSelector(contentType, alertId)
                    .done(function (versionCode, status) {
                        tools.navigate("/sys/console/sitealerts/edit/" + alertId + "?version=" + versionCode)
                    });
            });

           
        }
    }
});