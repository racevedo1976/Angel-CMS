$.mediaBrowser = function (options) {
    var defaults = {
        title: "Media Browser",
        multi: false,
        showImages: true,
        showVideos: false,
        allowedExtensions: []
    },
        template = [
            '<div class="media-browser modal-dialog">',
              '<div class="modal-content modal-layout">',
                '<div class="media-browser-heading">',
                    '<span class="media-browser-title"></span>',
                '</div>',
                '<div class="row media-browser-body">',
                    '<div class="col-sm-3">',
                        '<div class="media-browser-folders"></div>',
                        '<div class="media-browser-results" style="display: inline">',
                            '<h3>Selection</h3>',
                            '<div class="media-browser-selection"></div>',
                            '<div class="media-browser-selection-cropbtn"></div>',
                        '</div>',
                    '</div>',
                     '<div class="col-sm-9">',
                        '<div class="media-browser-files"></div>',
                     '</div>',
                 '</div>',
                '<div class="media-browser-footer">',
                    '<div class="clearfix">',
                        '<div class="media-browser-commands pull-right">',
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
                if ((options.showImages && file.contentType == "picture") || 
                    (options.showVideos && file.contentType == "film")) {
                    fileMappings[path +file.name]= file;
                }
                
            });
        }
    }

    function treeFolderSelect(e) {
        
        var folder = e.sender.dataItem(e.node);
        currentFolderNodeItem = e.node;
        var kendoBrowser = $modal.find(".media-browser-files").getKendoImageBrowser();

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

    function invokeCropper(id) {
        var $imgBtn = $(this);
        var id = $imgBtn.data().id;

        //alert(id);
        $.imageCropper({
            imageId: id,
            driveUrl: options.driveUrl
        }).done(function (result) {
            
            var $container = $modal.find(".media-browser-selection");
            var $cropBtnContainer = $modal.find(".media-browser-selection-cropbtn");

            var $img = $("<img/>")
                .attr("src", result.croppedUrl)
                .attr("data-index", selectedFiles.length)
                .on("dblclick", removeSelection);

            var $cropBtn = $("<a/>")
              .append("<i class='fa fa-crop'></i>")
              .attr("data-id", result.id)
              .on("click", invokeCropper);

            $container.empty();
            $cropBtnContainer.empty();

            $container.append($img).parent().show();
            $cropBtnContainer.append($cropBtn);

            selectedFiles[0].croppedUrl = result.croppedUrl;
            selectedFiles[0].isCropped = true;
        });
    };

    // Init
    options = $.extend({}, defaults, options);

    //get a unique local copy of options
    mediaBrowserOptions = options;

    $modal
        .css({ display: "block" })
        .append(template.join(''))
        .prependTo(document.body)
        .find(".media-browser-title")
        .html(options.title);

    if (options.multi === true) {
        $modal.find(".media-browser").addClass("media-browser-multi");
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

    $modal.find(".media-browser-folders").kendoTreeView({
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
                        
            this.trigger("select", {node: selectedFolder});

        }
    });

    var imageBrowser = $modal.find(".media-browser-files").kendoImageBrowser({
        transport: {
            read: function (options) {
                var path = options.data.path;
                var folder = branchMappings[path];

                if (folder === undefined) {
                    options.error("Invalid folder path");
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
                            if ((mediaBrowserOptions.showImages && file.contentType == "picture") ||
                                (mediaBrowserOptions.showVideos && file.contentType == "film")) {
                                file.url = mediaBrowserOptions.driveUrl + '/download/?id=' + file.id;
                                file.thumbnUrl = mediaBrowserOptions.driveUrl + '/download/thumbnail/' + file.id;
                                file.imageUrl = mediaBrowserOptions.driveUrl + '/documents/' + file.id + file.extension;
                                file.documentId = file.id;
                                file.isCropped = false;
                                file.croppedUrl = '';
                                filteredFileList.push(file);
                            }
                        });

                        buildFileMappings(filteredFileList, path);
                        options.success(filteredFileList);
                    });
                }
            },
            thumbnailUrl: function (path, name) {
                return options.driveUrl + '/download/thumbnail/' + encodeURIComponent(fileMappings[path + decodeURIComponent(name)].id) + "?tz=" + fileMappings[path + decodeURIComponent(name)].createdDateString;
            },
            imageUrl: function (path) {
                //note: returning image object, not url
                return fileMappings[path];
            },
            uploadUrl: "/api/user/library/doc?folderId=" + folderIdToDropFile,
        },
        change: function (e) {
            var image = e.sender.value();
            var $container = $modal.find(".media-browser-selection");
            var $cropBtnContainer = $modal.find(".media-browser-selection-cropbtn");
            var $img = $("<img/>")
                .attr("src", options.driveUrl + '/download/thumbnail/' + image.id)
                .attr("data-index", selectedFiles.length)
                .on("dblclick", removeSelection);

            var $cropBtn = $("<a/>")
               .append("<i class='fa fa-crop'></i>")
               .attr("data-id", image.id)
               .on("click", invokeCropper);

            if (!multiSelectMode) {
                $container.empty();
                $cropBtnContainer.empty();

                selectedFiles = [];

                selectedFiles.push(image)

                $container.append($img).parent().show();
                $cropBtnContainer.append($cropBtn);
            }



        },
    });

    imageBrowser = $modal.find(".media-browser-files").data("kendoImageBrowser");
    if (options.allowedExtensions.length > 0) {
        imageBrowser.options.fileTypes = "*" + options.allowedExtensions.join(",*"); // e.g. "*.png,*.gif,*.jpg,*.jpeg,*.mov";
    }
    
    var upload = $modal.find("[data-role=upload]").data("kendoUpload");
    if (options.allowedExtensions.length > 0) {
        upload.options.validation.allowedExtensions = options.allowedExtensions;
    }
    upload.bind("upload",
        function(e) {
            $modal.find(".k-upload-status").css("cssText", "display: inline !important;");
        });
    upload.bind("complete",
        function(e) {

            $modal.find(".k-upload-status").css({ display: "inline" });
            var treeView = $modal.find(".media-browser-folders").data("kendoTreeView");
            var selectedItem = treeView.dataSource.get(folderIdToDropFile);
            var selectedFolder = treeView.findByUid(selectedItem.uid);
            treeView.expand(selectedFolder);
            treeView.select(selectedFolder);

            treeView.trigger("select", { node: selectedFolder });
        });

    if (options.multi === true) {
        var dataSource = $modal.find(".media-browser-files").data("kendoImageBrowser").dataSource;
        $modal.find(".k-listview").kendoListView({
            selectable: "multiple",
            change: function () {
                var data = dataSource.view();
                selectedFiles = $.map(this.select(), function (item) {
                    return data[$(item).index()];
                });

                $modal.find(".media-browser-selection").html("Selected items: " + selectedFiles.length)

            },
        });

    }

    // Init Buttons
    $modal.find(".media-browser-commands [name=select]")
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

    $modal.find(".media-browser-commands [name=cancel]")
        .on("click", function (event) {
            // just close the modal w/o resolving the task
            $modal.remove();
        });

    // Return promise
    return task.promise();
}
