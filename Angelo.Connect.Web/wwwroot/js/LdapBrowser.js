$.ldapBrowser = function (options) {
    var defaults = {
        title: "Ldap Explorer",
        directoryId: '',
        poolId: ''
    },
      template = [
          '<div class="ldap-browser modal-dialog">',
            '<div class="modal-content modal-layout">',
              '<div class="ldap-browser-heading modal-header">',
                  '<span class="ldap-browser-title"></span>',
              '</div>',
              '<div class="row ldap-browser-body modal-body">',
                  
                  '<div class="col-sm-6">',
                      '<h4>Groups</h4>',
                      '<div class="ldap-browser-directories"></div>',
                  '</div>',
                  '<div class="col-sm-6">',
                       '<h4>Attributes</h4>',
                      '<div class="ldap-browser-attributes"></div>',
                  '</div>',
               '</div>',
              '<div class="ldap-browser-footer modal-footer">',
                  '<div class="clearfix">',
                      '<div class="ldap-browser-commands pull-right">',
                          '<button name="select" class="btn btn-default">&nbsp; Ok &nbsp;</button>',
                          '<button name="cancel" class="btn btn-default">Cancel</button>',
                      '</div>',
                  '</div>',
              '</div>',
           '</div>',
          '</div>'
      ],
      $modal = $('<div class="modal modal-layout" style="z-index:1070"></div>'),
      task = $.Deferred();
    var selectedLdapGroup;
    // Init
    var ldapOptions = $.extend({}, defaults, options);

    //display the modal
    $modal
       .css({ display: "block" })
       .append(template.join(''))
       .prependTo(document.body)
       .find(".ldap-browser-title")
       .html(options.title);

    //
    function treeFolderSelect(e) {

        var ldapObject = e.sender.dataItem(e.node);

        selectedLdapGroup = ldapObject;
        //debugger;
        $.get("/sys/corp/api/ldap/entry/attr?searchDn=" + ldapObject.distinguishedName + "&directoryId=" + options.directoryId).done(function (attr) {
            //debugger;
            if (attr.length > 0) {
                $modal.find(".ldap-browser-attributes").kendoGrid({
                    dataSource: {
                        data: attr,
                        schema: {
                            model: {
                                fields: {
                                    key: { type: "string" },
                                    value: { type: "string" }
                                }
                            }
                        },
                        pageSize: 20
                    },
                    height: 550,
                    scrollable: true,
                    sortable: true,
                    filterable: false,
                    columns: [

                        { field: "key", title: "Attribute Name" },
                        { field: "value", title: "value" }
                    ]
                });
            } else {
                $modal.find(".ldap-browser-attributes").html("No Attributes Available.")
            }
            //display the attr

        });
      
    };


    ///  Directory init
    //// Init Kendo
    var homogeneous = new kendo.data.HierarchicalDataSource({
        schema: {
            model:
                { fields: {} }
        },
        transport: {
            read: {
                url: "/sys/corp/api/ldap?directoryId=" + options.directoryId + "&poolId=" + options.poolId   // "/api/library/folders"
            }
        }
    });

    $modal.find(".ldap-browser-directories").kendoTreeView({
        dataTextField: "distinguishedName",
        //autoBind: true,
        loadOnDemand: true,
        dataSource: homogeneous,
        select: treeFolderSelect,
        dataBound: function (e) {

            //var rootFolder = this.dataSource.at(0);
            //var selectedFolder = this.findByUid(rootFolder.uid);
            //this.expand(selectedFolder);
            //this.select(selectedFolder);

            //this.trigger("select", { node: selectedFolder });

        }
    });


    // Init Buttons
    $modal.find(".ldap-browser-commands [name=select]")
        .on("click", function (event) {
            //var files = selectedFiles;

            //if (!multiSelectMode)
            //    files = files.length ? files[0] : undefined;
            var ldapEntry = selectedLdapGroup;

            var result = task.resolve(ldapEntry);

            $.isPromise(result)
                ? result.done(function () {
                    $modal.remove();
                })
                : $modal.remove();
        });

    $modal.find(".ldap-browser-commands [name=cancel]")
        .on("click", function (event) {
            // just close the modal w/o resolving the task
            $modal.remove();
        });

    //return promise
    return task.promise();
}