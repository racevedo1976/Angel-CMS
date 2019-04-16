$.documentBrowser = function (options) {
    var defaults = {
        title: "Document Browser",
        multi: false,
        allowedExtensions: []
    },
        template = [
            '<div class="document-browser modal-dialog">',
              '<div class="modal-content modal-layout">',
                '<div class="document-browser-heading">',
                    '<span class="document-browser-title"></span>',
                '</div>',
                '<div class="row document-browser-body">',
                    '<div class="col-sm-3">',
                        '<div class="document-browser-folders"></div>',
                        '<div class="document-browser-results" style="display: inline">',
                            '<h3>Selection</h3>',
                            '<div class="document-browser-selection"></div>',                           
                        '</div>',
                    '</div>',
                     '<div class="col-sm-9">',
                         '<div class="document-browser-upload" style="left: 300px; position: absolute;display: none;" id="document-browser-upload">',


                                '<div class="jumbotron">',
                                    '<div class="dropzoneForm dropzone" id="docActionDropZone">',
                                        '<div class="dz-message dz-default">',
                                            ' <span>Drag and drop or click here to select files for upload</span>',
                                        '</div>',
                                    '</div>',
                                '</div>',

                        '</div>',
                        '<div class="document-browser-upload-area" id="document-browser-upload-area">',
                            '<div class="document-browser-files"></div>',
                           
                        '</div>',
                     '</div>',
                 '</div>',
                '<div class="document-browser-footer">',
                    '<div class="clearfix">',
                        '<div class="document-browser-commands pull-right">',
                            '<button name="select" class="btn btn-default">&nbsp; Ok &nbsp;</button>',
                            '<button name="cancel" class="btn btn-default">Cancel</button>',
                        '</div>',
                    '</div>',
                '</div>',
             '</div>',
            '</div>'
        ],
        $modal = $('<div class="modal modal-layout" style="z-index:1070"></div>'),
        task = $.Deferred(),
        multiSelectMode = options.multi;

    var fileMappings;
    var branchMappings = { "": { id: "" } };
    var selectedFiles = [];
    var mediaBrowserOptions;
    var folderIdToDropFile = "";
    var currentFolderNodeItem;
   
    function addThumbnailTitles() {
        //add title/tooltip for each image in the list
        $("ul.k-listview li").each(function (index) {
            $(this).attr("title", $(this).text());
        });
    }

    function buildBranchMappings(ds, leaf) {
        var node = leaf.parent(), stack = [], path = "";

        stack.push(leaf);
        while (node = node.parent()) {
            // node > array > node
            if (node !== undefined) {
                stack.push(node);
                node = node.parent();
            }
        }

        branchMappings = { "": { id: "" } };
        while (node = stack.pop()) {
            path += node.title + "/"
            branchMappings[path] = node;
        }

        return path;
    }

    function buildFileMappings(files, path) {
        fileMappings = {};
        if ($.isArray(files)) {
            files.forEach(function (file) {
                var addToBrowser = false;
                fileMappings[path + file.name] = file;
            });
        }
    }

    function treeFolderSelect(e) {

        var folder = e.sender.dataItem(e.node);
        currentFolderNodeItem = e.node;
        var kendoBrowser = $modal.find(".document-browser-files").getKendoFileBrowser();

        var path = buildBranchMappings(this.dataSource, folder);

        kendoBrowser.path(path);
        kendoBrowser.breadcrumbs.value(path);

        //init titles on thumbnails
        window.setTimeout(function () { addThumbnailTitles(); }, 1000);
    };

    function removeSelection() {
        var $img = $(this);

        selectedFiles.splice($img.data().index, 1, []);
        $img.remove();
    }

    // Init
    options = $.extend({}, defaults, options);

    //get a unique local copy of options
    mediaBrowserOptions = options;

    $modal
        .css({ display: "block" })
        .append(template.join(''))
        .prependTo(document.body)
        .find(".document-browser-title")
        .html(options.title);

    if (options.multi === true) {
        $modal.find(".document-browser").addClass("document-browser-multi");
    }


    //// Init Kendo
    var homogeneous = new kendo.data.HierarchicalDataSource({
        schema: {
                model:
                    { fields: {} }
            },
        transport: {
            read: {
                url: "/sys/library/api/media/folders?userId=" + options.userId     // "/api/library/folders"
            }
        }
    });

    $modal.find(".document-browser-folders").kendoTreeView({
        dataTextField: "title",
        //autoBind: true,
        loadOnDemand: true,
        dataSource: homogeneous,
        select: treeFolderSelect,
        dataBound: function (e) {

            var rootFolder = this.dataSource.at(0);
            var selectedFolder = this.findByUid(rootFolder.uid);
            this.expand(selectedFolder);
            this.select(selectedFolder);

            this.trigger("select", { node: selectedFolder });

        }
    });


    $modal.find(".document-browser-files").kendoFileBrowser({
        fileTypes: "*.*",
        transport: {
            read: function (options) {
                var path = options.data.path;
                var folder = branchMappings[path];

                if (folder === undefined) {
                    options.error("Invalid folder path")
                }
                else {
                    var url = "/sys/library/api/media/files?userId=" + mediaBrowserOptions.userId + "&id=" + folder.id;
                    this.options.uploadUrl = "/api/user/library/doc?folderId=" + folder.id;
                    folderIdToDropFile = folder.id;

                    $modal.find("[data-role=upload]").data("kendoUpload").options.async.saveUrl =
                        "/api/user/library/doc?folderId=" + folderIdToDropFile;
                    
                    $.get(url).done(function (files) {
                        var filteredFileList = [];
                        $.each(files, function (i, file) {
                            file.url = mediaBrowserOptions.driveUrl + '/documents/' + file.id + file.extension;
                            filteredFileList.push(file);
                        });

                        buildFileMappings(filteredFileList, path);
                        options.success(filteredFileList);
                    });
                }
            },
            fileUrl: function (path) {
                //note: returning file object, not url
                return fileMappings[path];
            },
            uploadUrl: "/api/user/library/doc?folderId=" + folderIdToDropFile,
        },
       
    });

    var upload = $modal.find("[data-role=upload]").data("kendoUpload");
    if (options.allowedExtensions.length > 0) {
        upload.options.validation.allowedExtensions = options.allowedExtensions;
    }
    upload.bind("upload",
        function (e) {
            $modal.find(".k-upload-status").css("cssText", "display: inline !important;");
           
            window.setTimeout(function() {
                $modal.find(".k-upload-files").fadeOut(200);
                $modal.find(".k-upload-files").html('');
            }, 3000);
    });
    upload.bind("complete",
        function(e) {
            
            $modal.find(".k-upload-status").css({ display: "inline" });
            var treeView = $modal.find(".document-browser-folders").data("kendoTreeView");
            var selectedItem = treeView.dataSource.get(folderIdToDropFile);
            var selectedFolder = treeView.findByUid(selectedItem.uid);
            treeView.expand(selectedFolder);
            treeView.select(selectedFolder);

            treeView.trigger("select", { node: selectedFolder });
        });

    
    if (options.multi === true) {
        var dataSource = $modal.find(".document-browser-files").data("kendoFileBrowser").dataSource;
        $modal.find(".k-listview").kendoListView({
            selectable: "multiple",
            change: function () {
                var data = dataSource.view();
                selectedFiles = $.map(this.select(), function (item) {
                    return data[$(item).index()];
                });

                $modal.find(".document-browser-selection").html("Selected items: " + selectedFiles.length)
                
            },
        });

    }
   

    // Init Buttons
    $modal.find(".document-browser-commands [name=select]")
        .on("click", function (event) {
            var files = selectedFiles;

            if (!multiSelectMode)
                files = files.length ? files[0]: undefined;

            var result = task.resolve(files);

            $.isPromise(result)
                ? result.done(function () {
                    $modal.remove();
                })
                : $modal.remove();
        });

    $modal.find(".document-browser-commands [name=cancel]")
        .on("click", function (event) {
            // just close the modal w/o resolving the task
            $modal.remove();
        });

    // Return promise
    return task.promise();
}