
function FileManagerViewModel() {
    // Data
    var self = this;

    self.libraryApiUrl = '';
    self.connectApiUrl = '/api/user/library/';

    //merge documents and folders in one collection
    self.getFileExplorerMergedDs = function () {
        var fileCollection = [];
        $.each(self.CurrentFolderViewModel.ChildFolders, function (i, folder) {
            fileCollection.push({
                id: folder.Id,
                title: folder.Title,
                name: folder.Title,
                type: folder.Title == "Trash" ? "Trash" : "Folder",
                size: "-"
            })
        });
        $.each(self.CurrentFolderViewModel.documents, function (i, document) {
            fileCollection.push({
                id: document.DocumentId,
                title: document.FileName,
                name: document.FileName,
                type: document.FileType,
                size: document.FileSize,
            })
        });
        return fileCollection;
    }

    // Variables
    self.CurrentFolderName = '';
    self.CurrentFolderId = '';
    self.CurrentFolderViewModel = {};
    self.breadCrumbs = [];
    self.allFolders = [];

    // Binding functions
    self.loadDataContext = function () {
        var url = self.connectApiUrl + 'folder?userId=' + encodeURIComponent(userId);
        var current = self.CurrentFolderId;
        if (current) url += '&id=' + encodeURIComponent(current);

       // $('#myTable #folders').empty();
       // $('#myTable #docs').empty();
        self.libraryService(url, 'GET', {}, function (folder) {
            folder.documents = self.getDocuments(folder);
        });
        self.getAvailableFolders();
       
    };
    self.getDocuments = function (folder) {
        var url = self.connectApiUrl + 'docs?folderId=' + encodeURIComponent(folder.Id);
        self.libraryService(url, 'GET', {}, function (documents) {
            self.CurrentFolderViewModel = folder;
            self.CurrentFolderViewModel.documents = documents;
            self.bindItems(folder);
        });
    };
    self.getSharedDocuments = function (folder) {
        var url = self.connectApiUrl + 'docs/shared?userId=' + encodeURIComponent(userId);
        self.libraryService(url, 'GET', {}, function (documents) {
            self.CurrentFolderViewModel = folder;
            self.CurrentFolderViewModel.documents = documents;
            self.bindItems(folder);
        });
    };

    self.attachEvents = function () {
        //event for double clicking on a folder. 
        //Drill down to folder details and changing folder view
        $("#myTable #folders tr").dblclick(function () {
            self.loadDataContext();
        });

        $("#myTable #docs tr").dblclick(function () {
            var documentId = 0;
            self.downloadDocument(documentId);
        });
    };

    // Utility functions
    self.libraryService = function (url, method, data, callback) {
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

    };

    // UI actions
    self.goToFolder = function (folder, folderId) {
        self.CurrentFolderName = folder;
        self.CurrentFolderId = folderId;
        //self.breadCrumbs = [];
        self.loadDataContext();
        
    };
    self.deleteSelected = function () {
        var foldersSelected = self.getSelectedFolders();
        var documentsSelected = self.getSelectedDocuments();

        $.each(foldersSelected, function (i, folderId) {
            self.libraryService(self.connectApiUrl + "folder/?id=" + encodeURIComponent(folderId), "DELETE", {}, function () {
                $('#confirmDeleteModal').modal('hide');
                self.loadDataContext();
            });
        });

        $.each(documentsSelected, function (i, doc) {
            self.libraryService(self.connectApiUrl + "doc/?id=" + encodeURIComponent(doc.id), "DELETE", {}, function () {
                $('#confirmDeleteModal').modal('hide');
                self.loadDataContext();
            });
        });

    };
    self.createNewFolder = function () {
        var newName = $("#folderNewName").val();
        var url = self.connectApiUrl + 'folder?name=' + newName + '&userId=' + encodeURIComponent(userId);
        var name = self.CurrentFolderId;
        if (name && name != '')
        {
            url += '&parentId=' + encodeURIComponent(name);
        }
        self.libraryService(url, 'POST', {}, function () {
            $("#myCreateNewFolderModal").modal("hide");
            self.loadDataContext();
        });
    };
    self.moveSelected = function () {
        var destinationId = $('#destinationFolder option:selected').val();
        var sourceId = self.CurrentFolderId;
        var foldersSelected = self.getSelectedFolders();
        var documentsSelected = self.getSelectedDocuments();

        $.each(foldersSelected, function (i, folderId) {
            var sameName = self.getSameName(folder.name, destinationItems);
            if (sameName && sameName != '') {
                if (sameName.itemType == 'folder') {
                    prompt('Unable to overwrite a folder with a document: ' + sameName + '.');
                    return;
                }
                if (sameName.itemType == 'doc') {
                    if (!confirm('Do you with to overwrite the existing document: ' + sameName + '?')) return;
                }
            }

            self.libraryService(self.connectApiUrl + "move/?folderId=" + encodeURIComponent(folderId) + "&sourceFolderId=" + encodeURIComponent(sourceId) + '&destinationFolderId=' + encodeURIComponent(destinationId), "PUT", {}, function () {
                $('#moveItemModal').modal('hide');
                self.loadDataContext();
            });
        })

        $.each(documentsSelected, function (i, doc) {
            self.libraryService(self.connectApiUrl + "doc/move/?documentId=" + encodeURIComponent(doc.id) + "&sourceFolderId=" + encodeURIComponent(sourceId) + "&destinationFolderId=" + encodeURIComponent(destinationId), "PUT", {}, function () {
                $('#moveItemModal').modal('hide');
                self.loadDataContext();
            })
        })
    };
    self.copySelected = function () {
        var destinationId = $('#destinationFolderCopy option:selected').val();
        var sourceId = self.CurrentFolderId;
        var foldersSelected = self.getSelectedFolders();
        var documentsSelected = self.getSelectedDocuments();
        var destinationItems = self.getFolderItems(userId, destinationId);   // Docs and Folders with itemType

        $.each(foldersSelected, function (i, folder) {
            var sameName = self.getSameName(folder.name, destinationItems);
            if (sameName && sameName != null){
                if (sameName.itemType == 'folder') {
                    if (!confirm('Do you with to overwrite the existing folder: ' + sameName + '?')) return;
                }
                if (sameName.itemType == 'doc') {
                    prompt('Unable to overwrite a document with a folder: ' + sameName + '.');
                    return;
                }
            }

            var url = self.connectApiUrl + "copy/?folderId=" + encodeURIComponent(folder.id) + "&sourceFolderId=" + encodeURIComponent(sourceId) + '&destinationFolderId=' + encodeURIComponent(destinationId);
            if (foldersSelected.length == 1 && documentsSelected.length == 0) url += '&newName=' + encodeURIComponent($('#copyItemNewName').val());
            self.libraryService(url, "PUT", {}, function () {
                $('#copyItemModal').modal('hide');
                self.loadDataContext();
            });
        })

        $.each(documentsSelected, function (i, doc) {
            var sameName = self.getSameName(doc.name, destinationItems);
            if (sameName && sameName != '') {
                if (!confirm('Do you with to overwrite the existing item: ' + sameName + '?')) return;
            }

            var url = self.connectApiUrl + "doc/copy/?documentId=" + encodeURIComponent(doc.id) + "&sourceFolderId=" + encodeURIComponent(sourceId) + "&destinationFolderId=" + encodeURIComponent(destinationId);
            if (foldersSelected.length == 0 && documentsSelected.length == 1) url += '&newName=' + encodeURIComponent($('#copyItemNewName').val());
            self.libraryService(url, "PUT", {}, function () {
                $('#copyItemModal').modal('hide');
                self.loadDataContext();
            })
        })
    };
    self.getFolderItems = function(userId, destinationId){
        var results = [];
        var url = self.connectApiUrl + 'folder?userId=' + encodeURIComponent(userId) + '&id=' + encodeURIComponent(destinationId);
        self.libraryService(url, 'GET', {}, function (folder) {
            $.each(folder.ChildFolders, function (i, item) {
                results.push({ name: item, itemType: 'folder' });
            });
            $.each(folder.documents, function (i, item) {
                results.push({ name: item, itemType: 'doc' });
            });
        });

        return results;
    };
    self.getSameName = function (name, items) {
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            if (item.name == name) return item;
        }
        
        return null;
    };
    self.getSelectedDocuments = function () {
        var documentsSelected = [];
        $('#documentExplorer div.documentItemSelected').each(function (i, item) {
            if (item.attributes['data-type'].value == "document") {
                documentsSelected.push({ id: item.attributes['data-id'].value, name: item.childNodes[3].innerText });
            }           
        });
        return documentsSelected;
    }
    self.getSelectedFolders = function () {
        var foldersSelected = [];
        $('#documentExplorer div.documentItemSelected').each(function (i, item) {
            if (item.attributes['data-type'].value == "folder") {
                foldersSelected.push({ id: item.attributes['data-id'].value, name: item.childNodes[3].innerText });
            }
        });
        return foldersSelected;
    };
    self.downloadDocument = function (id) {
        var url = self.libraryApiUrl + "download?id=" + encodeURIComponent(id);
        window.open(url);
    };
    self.downloadSelected = function () {
        var folders = self.getSelectedFolders();
        var documents = self.getSelectedDocuments();

        var folderIds = folders.join();
        var documentIds = documents.map(function (doc) {
            return doc.id;
        }).join();
        var targetId = self.CurrentFolderId;
        var isSingleDoc = folders.length == 0 && documents.length == 1;
        if (isSingleDoc){
            self.downloadSingle(documents[0].name, self.libraryApiUrl + 'download?id=' + encodeURIComponent(documentIds));
        }
        else{
            self.downloadZip(self.libraryApiUrl + 'download/zipasync?ownerId=' + encodeURIComponent(userId) + '&folderIds=' + encodeURIComponent(folderIds) + '&documentIds=' + encodeURIComponent(documentIds) + '&targetFolderId=' + encodeURIComponent(targetId));
        }
        //window.open(url);
    };
    self.downloadSingle = function (fileName, url) {
        self.libraryService(url, "GET", {}, function (data) {
            // At this point, we have a Document entry, but not a file entry. For that, we need to set up a polling setup

            $('#dllink').text(fileName);
            $('#dllink').attr('href', url);

            $('#dlstart').addClass('hidden');
            $('#dlhint').addClass('hidden');
            $('#dllink').removeClass('hidden');
        });
    }
    self.downloadZip = function (url) {
        self.libraryService(url, "GET", {}, function (data) {
            // At this point, we have a Document entry, but not a file entry. For that, we need to set up a polling setup
            $('#dlhint').text('Downloading ' + data.fileName + '...');
            $('#dlstart').removeClass('hidden');

            $('#dlhint').removeClass('hidden');
            $('#dllink').addClass('hidden');
            $('#dllink').text(data.fileName);
            var url = self.libraryApiUrl + "download?id=" + encodeURIComponent(data.documentId);
            $('#dllink').attr('href', url);

            self.loadDataContext(); // Shows the DB entry
            self.pollZip(data.documentId);
        });
    };
    self.pollZip = function (documentId) {
        var url = self.libraryApiUrl + 'downloadSize?id=' +encodeURIComponent(documentId);
        self.libraryService(url, "GET", {}, function (data) {
            if (data && data > 0) {
                $('#dlhint').text("Press 'Start' to begin downloading.");

                $('#dlhint').addClass('hidden');
                $('#dllink').removeClass('hidden');

                self.loadDataContext(); // Shows the updated file size
            }
            else {
                setTimeout(self.pollZip(documentId), 5000);
            }
        });
    };

    self.renameSelected = function () {
        var foldersSelected = self.getSelectedFolders();
        var documentsSelected = self.getSelectedDocuments();
        var newName = $("#renameItemNewName").val();

        $.each(foldersSelected, function (i, folderId) {
            self.libraryService(self.connectApiUrl + 'rename?id=' + encodeURIComponent(folderId) + '&newName=' + encodeURIComponent(newName), 'PUT', {}, function () {
                $("#renameItemNameModal").modal("hide");
                self.loadDataContext();
            });
        });
        
        $.each(documentsSelected, function (i, doc) {
            self.libraryService(self.connectApiUrl + "doc/?documentId=" + encodeURIComponent(doc.id) + "&newName=" + encodeURIComponent(newName), "PUT", {}, function () {
                $("#renameItemNameModal").modal("hide");
                self.loadDataContext();
            })

        });       
    };    

    self.toViewModel = function (folder) {
        if (folder == null) return null;
        return {
            id: item.Id,
            title: item.Title,
            name: item.Title,
            type: "Folder",
            parentFolder: self.toViewModel(item.ParentFolder)
        }
    }
    self.getAvailableFolders = function () {
        self.libraryService(self.connectApiUrl + 'folders?userId=' + encodeURIComponent(userId), 'GET', {}, function (data) {
            data = data.sort(function (a, b) {
                if (a.path < b.path) return -1;

                if (a.path > b.path) return 1;

                return 0;
            });
            
            self.allFolders= [];
            self.allFoldersExceptMe = [];
            for (var i = 0; i < data.length; i++) {
                var item = self.toViewModel(data[i]);
                self.allFolders.push(item);

                var folderId = self.CurrentFolderId;
                if (!folderId && item.ParentId == null) continue;  // Exclude self(implicit root)
                if (folderId && item.Id == folderId) continue;           // Exlude self

                self.allFoldersExceptMe.push(item);
            }

            // Moved this here due to need to synchronize this with getAvailableFolders
            self.bindMoveFolders();
            self.bindCopyFolders();
        });
    };
   
   
    self.getFolderPath = function (folder) {
        var result = '';

        if (folder) {
            var parent = self.getFolderPath(folder.parentFolder);
            if (parent == '/') parent = '';

            result = parent + '/' + folder.title;
        }

        return result;
    };
    // Binding
    self.bindMoveFolders = function () {
        $('#destinationFolder').empty();
        for (var i = 0; i < self.allFoldersExceptMe.length; i++) {
            var folder = self.allFoldersExceptMe[i];
            $('#destinationFolder').append($('<option value="' + folder.Id + '">' + self.getFolderPath(folder) + '</option>'));
        }
    };
    self.bindCopyFolders = function () {
        $('#destinationFolderCopy').empty();
        for (var i = 0; i < self.allFoldersExceptMe.length; i++) {
            var folder = self.allFoldersExceptMe[i];
            $('#destinationFolderCopy').append($('<option value="' + folder.Id + '">' + self.getFolderPath(folder) + '</option>'));
        }
    };
    self.bindItems = function (folder) {
        self.CurrentFolderName = folder.Title;
        self.CurrentFolderId = folder.Id;

        self.bindTrash();

        self.bindDocExplorer()

        $('#panelFolderDetails').trigger('folderViewChange', {
            id: self.CurrentFolderId // any argument
        });
        //self.bindFolders(folder.folders, $('#myTable #folders'));
        //self.bindDocs(folder.documents, $('#myTable #docs'));

        
    };

    //trash
    self.bindTrash = function () {
        self.libraryService(self.connectApiUrl + 'getTrashFolder?userId=' + encodeURIComponent(userId), 'GET', {}, function (data) {
            if (data) {
                $("#trashBin").attr("onclick", "raiseOpenFolderEvent('" + data.title + "','" + data.id + "')")
            }
        });
    }

    //folder breadcrumbs - deprecated
    self.bindCrumbs = function (folder) {
        //self.initCrumbs(path, parentFolderId);
        var currentBreadIndex = -1;
        var totalBreadCrumbItems = self.breadCrumbs.length -1;
        $.each(self.breadCrumbs, function (i, breadItem) {
            if (breadItem.id == folder.id) {
                currentBreadIndex = i;   //get the index of the current Breadcrumb
            };
        });

        if (currentBreadIndex > -1) {
            //its already in the breadcrum list lets remove the subs
            for (var iterations = 1; iterations <= (totalBreadCrumbItems - currentBreadIndex) ; iterations++) {
                self.breadCrumbs.pop();
            };
        } else {
            //if not in the breadcrumb list then add.
            self.breadCrumbs.push({ id: folder.id, title: folder.title })
        }

        // Bind parents
        var parents = $('#parentTrail');
        parents.empty();
        for (var i = 0; i < self.breadCrumbs.length -1; i++) {  //it minus 1, so we never display the current folder here
            var folderName = self.breadCrumbs[i].title;
            var folderId = self.breadCrumbs[i].id;
            var html = '<li style="float: left;">' +
                '<button class="btn btn-link" onclick="raiseOpenFolderEvent(\'' + folderName + '\',\'' + folderId + '\')">' +
                '<span class="menu-root site-menu">' + (folderName == '' ? 'My Root' : folderName) + '</span>' +
                '</button><span class="btn-link menu-root site-menu">></span></li>';
            parents.append(html);
        }

        // Bind self
        var name = self.CurrentFolderName;
        if (!name || name == '') {
            name = "My Root";
        }
        $('#currentFolderName').text(name);
    };
    self.GetBreadcrumbsPath = function (id) {
        self.libraryService(self.connectApiUrl + 'folderTreePath?userId=' + encodeURIComponent(userId) + "&id=" + encodeURIComponent(id), 'GET', {}, function (data) {
            //To KendoMenu data structure
            var parents = $('#parentTrail');
            parents.empty();
           
            parents.append(self.generateBreadcrumbs(data,"",id));
            
            // Bind self
            var name = self.CurrentFolderName;
            if (!name || name == '') {
                name = "My Root";
            }
            parents.append('<li style="float: left;padding: 6px 12px; float: left;">' + name + '</li>')
           // $('#currentFolderName').text(name);
        });
    };
    self.generateBreadcrumbs = function (currfolder, crumbsPath, selectedFolderId) {
        var crumbsPath1 = '';
        $.each(currfolder, function (i, folder) {
            if (folder.Id != selectedFolderId) {

                crumbsPath1 += '<li style="float: left;">' +
                        '<button class="btn btn-link" onclick="raiseOpenFolderEvent(\'' + folder.Title + '\',\'' + folder.Id + '\')">' +
                        '<span class="menu-root site-menu">' + (folder.Title == '' ? 'My Root' : folder.Title) + '</span>' +
                        '</button><span class="btn-link menu-root site-menu">></span></li>';

                //if (folder.ChildFolders && folder.ChildFolders.length > 0) {

                //    crumbsPath1 += self.generateBreadcrumbs(folder.ChildFolders[0], crumbsPath, selectedFolderId)

                //}
            }

        });
        
        return crumbsPath1;
    }

    self.bindDocExplorer = function () {
        var sourceData = self.getFileExplorerMergedDs();
        var ds = new kendo.data.DataSource({
            data: sourceData,
        });
        var isEmtpy = sourceData.length == 0;

        var docExplorer = $("#documentExplorer").data('kendoListView');
        if (docExplorer) {
            docExplorer.setDataSource(ds);
            docExplorer.dataSource.read();
            docExplorer.refresh();
        } else {
            $("#documentExplorer").kendoListView({
                dataSource: ds,
                template: kendo.template($("#explorerTemplate").html())
            });
        }

        if (isEmtpy)
            $("#documentExplorer").html("<div style='text-align:center;padding-top:4em;'>Folder is empty. Use the 'Create' button above to start uploading files.</div>")

    }
    
    //Menu Tree functions
    self.InitMenuTree = function () {
        self.getMenuTreeData();
    };
    self.getMenuTreeData = function () {
        self.libraryService(self.connectApiUrl + 'folderTree?userId=' + encodeURIComponent(userId), 'GET', {}, function (data) {
            //To KendoMenu data structure
            var menuDataSource = [];
            $.each(data, function (i, folder) {
                menuDataSource.push(self.toMenuTreeView(folder));
            });
            self.bindMenu(menuDataSource);
        });
    };
    self.toMenuTreeView = function (folder) {
        var menu = [];
        var menuItems = [];
        var hasItems = false;
        var folderNode = { text: folder.Title == "" ? "My Root" : folder.Title, id: folder.Id };

        $.each(folder.ChildFolders, function (i, subFolder) {
            if (subFolder.Title != "Trash") {
                hasItems = true;
                menuItems.push(self.toMenuTreeView(subFolder))
            }   
        });
        if (hasItems) folderNode.items = menuItems;
        //menu.push(folderNode);
        return folderNode;
    };
    self.bindMenu = function (menuSource) {

        var libraryMenu = new kendo.data.HierarchicalDataSource({
            data: menuSource
        });

        var folderTreeView = $("#libraryMenu").data("kendoTreeView");
        if (folderTreeView) {
            folderTreeView.setDataSource(libraryMenu);
            folderTreeView.dataSource.read();
           // folderTreeView.refresh();
        } else {
            $("#libraryMenu").kendoTreeView({
                template: kendo.template($("#treeview-template").html()),
                dataSource: libraryMenu,
                select: function (e) {

                    //var nodes = $(e.node).parentsUntil($("#libraryMenu"), "li");
                    //console.log("change ", e.node);
                    //$.each(nodes, function (i, nod) {
                    //    var dd = $("#libraryMenu").data("kendoTreeView").dataItem(nod);
                    //    alert(dd.id);
                    //});
                    var data = $("#libraryMenu").data("kendoTreeView").dataItem(e.node);
                    //console.log("change ", data.id);

                    $('#libraryMenu').trigger('openFolder', {
                        id: data.id, // any argument
                        name: data.text
                    });

                }
            });

            //autoexpand the first node - there is an issue when programmatically selecting a node
            //nodes that not shown at first time (after init), are not really created yet until expanded - weird stuff
            if (menuSource && menuSource.length > 0 ) {
                self.expandFolderNode(menuSource[0].id);
            }
        }


    };
    self.expandFolderNode = function (id) {
        //var treeView = $("#libraryMenu").data("kendoTreeView");
        //treeView.expand(treeView.findByText(name))

        var treeView = $("#libraryMenu").data("kendoTreeView");
        var selectedItem = treeView.dataSource.get(id);
        var selectedFolder = treeView.findByUid(selectedItem.uid);
        treeView.expand(selectedFolder);
    }
    self.selectMenuFolder = function (id) {
        var treeView = $("#libraryMenu").data("kendoTreeView");
        var selectedItem = treeView.dataSource.get(id);
        var selectedFolder = treeView.findByUid(selectedItem.uid);
        treeView.select(selectedFolder);
    }
}


(function ($, window) {

    $.fn.contextMenu = function (settings) {

        return this.each(function () {

            // Open context menu
            $(this).on("contextmenu", function (e) {
                // return native menu if pressing control
                if (e.ctrlKey) return;

                 //open menu
                var $menu = $(settings.menuSelector)
                    .data("invokedOn", $(e.target))
                    .show()
                    .css({
                        position: "absolute",
                        left: getMenuPosition((e.pageX + 20) - ($(window).width() * .17), 'width', 'scrollLeft'),
                        top: getMenuPosition((e.pageY) - 110, 'height', 'scrollTop')
                    })
                    .off('click')
                    .on('click', 'a', function (e) {
                        $menu.hide();

                        var $invokedOn = $menu.data("invokedOn");
                        var $selectedMenu = $(e.target);

                        settings.menuSelected.call(this, $invokedOn, $selectedMenu);
                    });

                return false;
            });

            //make sure menu closes on any click
            $('body').click(function () {
                $(settings.menuSelector).hide();
            });
        });

        function getMenuPosition(mouse, direction, scrollDir) {
            var win = $(window)[direction](),
                scroll = $(window)[scrollDir](),
                menu = $(settings.menuSelector)[direction](),
                position = mouse;// + scroll;

            // opening menu would pass the side of the page
            if (mouse + menu > win && menu < mouse)
                position -= menu;

            return position;
        }

    };
})(jQuery, window);

var fmVM = new FileManagerViewModel();

$(document).ready(function () {
    
    //File Upload response from the server
   

    //SUBSCRIBE TO event
    var subscriptionDocuments = $('#panelFolderDetails').on('documentUpdated', function (event, obj) {
        // Do something now that the event has occurred
        fmVM.loadDataContext();
        //libraryService(apiUrl + "doc?id=" + encodeURIComponent(obj.id), "Get", {}, applyDocumentModelToForm);

    });
    //SUBSCRIBE TO event
    var subscriptionFolders = $('#panelFolderDetails').on('folderUpdated', function (event, obj) {
        // Do something now that the event has occurred
        fmVM.loadDataContext();
       
        //Reload the folder treeview
        fmVM.getMenuTreeData();

        //selectMenuFolder
        //fmVM.selectMenuFolder(obj.id)

    });

    //SUBSCRIBE TO event
    var subscriptionFolders2 = $('#libraryMenu').on('openFolder', function (event, obj) {
        //set model selections
        fmVM.CurrentFolderName = obj.name;
        fmVM.CurrentFolderId = obj.id;

        //Go to folder - opens folder contents and display on main screen
        fmVM.goToFolder(obj.name, obj.id)

        //// build breadcrumbs based on the folder selected
        fmVM.GetBreadcrumbsPath(obj.id);
        
        //selectMenuFolder
        if (obj.name != "Trash") {
            fmVM.selectMenuFolder(obj.id)

            //autoexpand the first node - there is an issue when programmatically selecting a node
            //nodes that not shown at first time (after init), are not really created yet until expanded - weird stuff
            //just making sure the sub folders are create by autoexpanding them
            fmVM.expandFolderNode(obj.id);
        }

        //open panel details for folder selection
        $('#panelFolderDetails').trigger('folderSelected', {
            id: obj.id // any argument
        });
    });

    var openShareEvent = $('#ShareDocuments').on('openFolder', function (event, obj) {
        //set model selections
        fmVM.CurrentFolderName = obj.name;
        fmVM.CurrentFolderId = obj.id;

        //alert('Open Share triggered.')
        var folder = {
            Id: obj.id,
            Title: obj.name
        }
        fmVM.getSharedDocuments(folder);
       
    })

    $("#ShareDocuments").dblclick(function () {

        $('#ShareDocuments').trigger('openFolder', {
            id: 'shared', // any argument
            name: 'Share Documents'
        });
    });
    
});


//Global Events list (sub/pub)
// - documentUpdated  -- editing document details
// - documentSelected -- selecting a document
// - folderUpdated    -- editing folder details
// - folderSelected   -- selecting a folder, not opening folder
// - folderViewChange -- 
// - openFolder       -- Open folder


//pub sub pattern
//var FileManagerEvents = (function () {
//    var topics = {};
//    var hOP = topics.hasOwnProperty;

//    return {
//        subscribe: function (topic, listener) {
//            // Create the topic's object if not yet created
//            if (!hOP.call(topics, topic)) topics[topic] = [];

//            // Add the listener to queue
//            var index = topics[topic].push(listener) - 1;

//            // Provide handle back for removal of topic
//            return {
//                remove: function () {
//                    delete topics[topic][index];
//                }
//            };
//        },
//        publish: function (topic, info) {
//            // If the topic doesn't exist, or there's no listeners in queue, just leave
//            if (!hOP.call(topics, topic)) return;

//            // Cycle through topics queue, fire!
//            topics[topic].forEach(function (item) {
//                item(info != undefined ? info : {});
//            });
//        }
//    };
//})();