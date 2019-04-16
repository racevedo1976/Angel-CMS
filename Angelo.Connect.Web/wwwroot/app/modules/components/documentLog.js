var apiUrl = "";

function libraryService(url, method, data, callback) {
    $.ajax({
        url: url,
        type: method,
        data: data,
        contentType: 'application/json',
        //xhrFields: {
        //    withCredentials: true
        //},
        success: function (data) {
            if (callback) {
                callback(data)
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
}

function applySummaryModel(summary) {
    $('#lastdl').html(summary.lastDownload ? summary.lastDownload : "Never");
    $('#lastdlother').html(summary.lastDownloadOther ? summary.lastDownloadOther : "Never");
    $('#lastwrite').html(summary.lastWrite ? summary.lastWrite : "Never");
    $('#lastwriteother').html(summary.lastWriteOther ? summary.lastWriteOther : "Never");
    $('#dlcount').html(summary.downloadCount);
    $('#dlcountother').html(summary.downloadCountOther);
    $('#readcount').html(summary.readCount);
    $('#readcountother').html(summary.readCountOther);
    $('#writecount').html(summary.writeCount);
    $('#writecountother').html(summary.writeCountOther);
}

function attachSubsPubEvents_DocLog() {
    var subscriptionDocument = $('#panelFolderDetails').on('documentSelected', function (event, obj) {
        // Do something now that the event has occurred
        libraryService(apiUrl + "doc/log?documentId=" + encodeURIComponent(obj.id), "Get", {}, function (data) {
            var ds = $('#docLog').data('kendoGrid');
            if (ds) {
                var dataSource = new kendo.data.DataSource({
                    data: data.events
                });
                ds.setDataSource(dataSource);
                ds.dataSource.read();
                ds.refresh();
            }
            else {
                $('#docLog').kendoGrid({
                    dataSource: data.events,
                    rowTemplate: kendo.template($('#logRowTemplate').html()),
                    columns: [{ title: 'Action', field: 'action' }, { title: 'Timestamp', field: 'created', format: '{0:g}' }]//, { title: 'User', field: 'userId' }]
                });
            }

            applySummaryModel(data.summary);
        });
    });
    var subscriptionFolder = $('#panelFolderDetails').on('folderSelected', function (event, obj) {
        //libraryService(apiUrl + "folder/log?documentId=" + encodeURIComponent(obj.id), "Get", {}, function (data) {
        var data = new Array();
        var ds = $('#docLog').data('kendoGrid');
        if (ds) {
            var dataSource = new kendo.data.DataSource({
                data: data
            });
            ds.setDataSource(dataSource);
            ds.dataSource.read();
            ds.refresh();
        }
        else {
            $('#docLog').kendoGrid({
                dataSource: data,
                rowTemplate: kendo.template($('#logRowTemplate').html()),
                columns: [{ title: 'Action', field: 'action' }, { title: 'Timestamp', field: 'created', format: '{0:g}' }]//, { title: 'User', field: 'userId' }]
            });
        }
    });
}
