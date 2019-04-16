var apiUrl = "";
var documentModel = {};
var folderModel = {};
var tagsCollection = {};
var tagElement = {};
var folderTagElement = {};

function GetTagNameById(tagId) {
    var tName = "";
    $.each(tagsCollection, function (i, tag) {
        if (tagId == tag.TagId) {
            tName = tag.TagName;
        }
    });
    return tName;
}
function GetTagIdByName(tagName) {
    var tId = "";
    $.each(tagsCollection, function (i, tag) {
        if (tagName == tag.TagName) {
            tId = tag.TagId;
        }
    });
    return tId;
}

function updateFolder() {
    folderModel.description = $("#folderDescription").val();
    folderModel.title = $("#folderName").val();
    libraryService(apiUrl + "folder/update", "POST", JSON.stringify(folderModel), function () {
        //TODO RAISE/PUB UPDATE MODEL EVENT
        $('#panelFolderDetails').trigger('folderUpdated', {
            refresh: true, // any argument
            id : folderModel.id
        });
    });

    //TODO RAISE/PUB UPDATE MODEL EVENT

};

function updateDocument() {
    //TODO SAVE MODEL
    documentModel.description = $("#fileDescription").val();
    documentModel.fileName = $("#fileName").val();

    libraryService(apiUrl + "doc/update", "POST", JSON.stringify(documentModel), function () {
        //TODO RAISE/PUB UPDATE MODEL EVENT
        $('#panelFolderDetails').trigger('documentUpdated', {
            refresh: true // any argument
        });
    });
};

function applyDocumentModelToForm(model) {
    documentModel = model;
    $("#panelDocDetails").removeClass("hidden");
    $("#panelNonSelectedFile").addClass("hidden");
    $("#panelFolderDetails").addClass("hidden");
    $("#fileName").val(model.FileName);
    $("#fileType").html(model.FileType);
    $("#fileDescription").val(model.Description);
    $("#fileId").val(model.DocumentId);
    $("#id").val(model.DocumentId);
    if (model.FileType == "picture") {
        $("#otherFilesIcon").addClass("hidden");
        $("#fileThumbnailSrc").removeClass("hidden");
        $("#fileThumbnailSrc").attr("src", fmVM.libraryApiUrl + "download/image?id=" + encodeURIComponent(model.DocumentId));

    } else {
        $("#fileThumbnailSrc").addClass("hidden");
        $("#otherFilesIcon").removeClass("hidden").addClass(GetDocumentClass(model.FileType)).addClass("fa-4x");
       
    }
    $("#fileSize").html(model.FileSize);
    $("#fileCreated").html(model.CreatedDateString);
    $("#fileOwner").html("me");
    $("#fileLocation").html(model.FileLocation);
    $("#headerFileIcon").removeClass();
    $("#headerFileIcon").addClass(getFileIconClass(model.FileType));

    //Clear tags
    tagElement.tagsinput('removeAll');
    if (model.tags) {
        $.each(model.tags, function (i, tag) {
            var tn = GetTagNameById(tag.tagId);
            tagElement.tagsinput('add', tn);
        });
    }

}

function applyFolderModelToForm(model) {
    folderModel = model;

    if (model.Title != "Trash") {
        $("#panelFolderDetails").removeClass("hidden");
        $("#panelNonSelectedFile").addClass("hidden");
        $("#panelDocDetails").addClass("hidden");
        $("#folderName").val(model.Title == '' ? "My Root" : model.Title);
        $("#folderType").html("Folder");
        $("#folderDescription").val(model.Description);
        $("#folderId").val(model.Id);
        //$("#fileThumbnailSrc").attr("src", apiUrl + "download/" + encodeURIComponent(model.id));
        //$("#fileSize").html(model.fileSize);
        $("#folderCreated").html(model.CreatedDateString);
        $("#folderOwner").html("me");
        //$("#fileLocation").html(model.fileLocation);

        //Clear tags
        folderTagElement.tagsinput('removeAll');
        if (model.Tags) {
            $.each(model.Tags, function (i, tag) {
                var tn = GetTagNameById(tag.tagId);
                folderTagElement.tagsinput('add', tn);
            });
        }
    } else {
        $("#panelNonSelectedFile").removeClass("hidden");
        $("#panelFolderDetails").addClass("hidden");
        $("#panelDocDetails").addClass("hidden");
    }
    
}

function getFileIconClass(fileType) {
    switch (fileType) {
        case "picture":
            return "fa fa-file-image-o";
            //break;
        case "application/octet-stream":
            return "fa fa-file-video-o";
            //break;
        case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
            return "fa fa-excel-o";
            //break;
        default:
            return "fa fa-file-o";
            //break;

    }

}

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
            //alert(data);
            //debugger;
            if (callback) {
                callback(data)

            }

        },
        error: function (xhr, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
}

function loadTagsForUser() {
    libraryService("/admin/User/Tags", "Get", {}, function (data) {
        tagsCollection = data;

        tagElement.tagsinput({
            typeahead: {
                afterSelect: function(val){ this.$element.val("");},
                source: $.map(data, function (t) {
                    return t.TagName
                })
            }
        });
        folderTagElement.tagsinput({
            typeahead: {
                afterSelect: function (val) { this.$element.val(""); },
                source: $.map(data, function (t) {
                    return t.TagName
                })
            }
        });

    })
}

function addTagToDocument(tagId) {
    var documentId = $("#fileId").val();
    libraryService(apiUrl + "doc/tag/?id=" + encodeURIComponent(documentId) + "&tagId=" + tagId, "POST", {});

}

function removeTagFromDocument(tagName) {
    var documentId = $("#fileId").val();
    var tagId = GetTagIdByName(tagName);
    libraryService(apiUrl + "doc/tag/?id=" + encodeURIComponent(documentId) + "&tagId=" + tagId, "DELETE", {
    });

}

function addTagToFolder(tagId) {
    var folderId = $("#folderId").val();

    libraryService(apiUrl + "folder/tag/?id=" + encodeURIComponent(folderId) + "&tagId=" + tagId, "POST", {});

}

function removeTagFromDocument(tagName) {
    var folderId = $("#folderId").val();
    var tagId = GetTagIdByName(tagName);

    libraryService(apiUrl + "folder/tag/?id=" + encodeURIComponent(folderId) + "&tagId=" + tagId, "DELETE", {
    });

}

function attachSubsPubEvents() {
    
    tagElement = $('#documentTags');
    loadTagsForUser();

    tagElement.on('itemAdded', function (event) {
        // event.item: contains the item
      
        libraryService("/admin/User/AddTag/?tagName=" + encodeURIComponent(event.item), "GET", {}, function (newTag) {
            tagsCollection.push(newTag);
            //Save new tag id to the document which was tagged.
            addTagToDocument(newTag.TagId);
        })
        tagElement.tagsinput('refresh');
        
    });

    tagElement.on('itemRemoved', function (event) {
        libraryService("/admin/User/DeleteTag/?tagName=" + encodeURIComponent(event.item), "GET")

        //Remove tag id to the document tag list if exists was tagged. 
        //event.item is the tag description
        removeTagFromDocument(event.item);

        //refresh local TAG collection
        loadTagsForUser();
    });

    folderTagElement = $('#folderTags');
    folderTagElement.on('itemAdded', function (event) {
        // event.item: contains the item

        libraryService("/admin/User/AddTag/?tagName=" + encodeURIComponent(event.item), "GET", {}, function (newTag) {
            tagsCollection.push(newTag);
            //Save new tag id to the document which was tagged.
            addTagToFolder(newTag.TagId);
        })
        tagElement.tagsinput('refresh');

    });

    folderTagElement.on('itemRemoved', function (event) {
        libraryService("/admin/User/DeleteTag/?tagName=" + encodeURIComponent(event.item), "GET")

        //Remove tag id to the document tag list if exists was tagged. 
        //event.item is the tag description
        removeTagFromFolder(event.item);

        //refresh local TAG collection
        loadTagsForUser();
    });

    var subscriptionDocument = $('#panelFolderDetails').on('documentSelected', function (event, obj) {
        updateMenuAvail();
        // Do something now that the event has occurred
        libraryService(apiUrl + "doc?id=" + encodeURIComponent(obj.id), "Get", {}, applyDocumentModelToForm);

    });

    var subscriptionFolder = $('#panelFolderDetails').on('folderSelected', function (event, obj) {
        updateMenuAvail();
        // Do something now that the event has occurred
        libraryService(apiUrl + "folder?id=" + encodeURIComponent(obj.id) + "&userId=" + userId, "Get", {}, applyFolderModelToForm);

    });

    var subscriptionFolder = $('#libraryMenu').on('openFolder', function (event, obj) {
        // Do something now that the event has occurred
        libraryService(apiUrl + "folder?id=" + encodeURIComponent(obj.id) + "&userId=" + userId, "Get", {}, applyFolderModelToForm);

    });
}


function getSelectedItemNames() {
    var names = '';
    $('#documentExplorer div.documentItemSelected div.document-name').each(function (i, item) {
        names += $(item).text() + ',';
    });
    names = names.substr(0, names.length - 1);
    if (names.length > 40) names = names.substr(0, 40) + '...';
    return names;
}

function updateMenuAvail() {
    var docs = $('#documentExplorer div.documentItemSelected[data-type="document"]');
    var folders = $('#documentExplorer div.documentItemSelected[data-type="folder"]')

    var length = docs.length + folders.length;
    switch (length) {
        case 0:
            $('#getfolders').addClass('hidden');
            $('#movefolders').addClass('hidden');
            $('#copyfolders').addClass('hidden');
            $('#renamefolder').addClass('hidden');
            $('#deletefolders').addClass('hidden');

            $('#getdocs').addClass('hidden');
            $('#movedocs').addClass('hidden');
            $('#copydocs').addClass('hidden');
            $('#renamedoc').addClass('hidden');
            $('#deletedocs').addClass('hidden');
            $('#sharedocs').addClass('hidden');
            
            break;
        case 1:
            $('#getfolders').removeClass('hidden');
            $('#movefolders').removeClass('hidden');
            $('#copyfolders').removeClass('hidden');
            $('#renamefolder').removeClass('hidden');
            $('#deletefolders').removeClass('hidden');

            $('#getdocs').removeClass('hidden');
            $('#movedocs').removeClass('hidden');
            $('#copydocs').removeClass('hidden');
            $('#renamedoc').removeClass('hidden');
            $('#deletedocs').removeClass('hidden');
            $('#sharedocs').removeClass('hidden');

            $('#deleteItemName').text(folders.length == 1 ? folders.text() : docs.text());
            $("#renameItemNewName").val(getSelectedItemNames());

            $('#copyItemNewName').val($('#documentExplorer div.documentItemSelected .document-name').text());
            $('#copyItemNewNamePanel').removeClass('hidden');
            break;
        default:
            $('#getfolders').removeClass('hidden');
            $('#movefolders').removeClass('hidden');
            $('#copyfolders').removeClass('hidden');
            $('#renamefolder').addClass('hidden');
            $('#deletefolders').removeClass('hidden');

            $('#getdocs').removeClass('hidden');
            $('#movedocs').removeClass('hidden');
            $('#copydocs').removeClass('hidden');
            $('#renamedoc').addClass('hidden');
            $('#deletedocs').removeClass('hidden');
            $('#sharedocs').removeClass('hidden');

            $('#copyItemNewName').val('');
            $('#copyItemNewNamePanel').addClass('hidden');
            break;
    }

    $('#deleteItemName').text(getSelectedItemNames());
}