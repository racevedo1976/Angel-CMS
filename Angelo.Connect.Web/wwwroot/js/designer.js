//----------------------------------------------------------------
// jQuery Plugins - Content Components
//----------------------------------------------------------------
$.fn.contentZone = function () {
    var $zone = this;

    function _makeDroppable(index) {
        var $dropTarget = $("<div></div>")
            .droppable({
                greedy: true,
                tolerance: "pointer", // fit, pointer, touch 
                accept: ".content-node, .designer-widget",
                over: function () {
                    $(this).addClass("active");
                },
                out: function () {
                    $(this).removeClass("active");
                },
                drop: function (e, ui) {
                    $(this).removeClass("active");

                    // prevent weird glitched double drops by locking for 1 second
                    if (designer.locked) return;

                    designer.locked = true;
                    window.setTimeout(function () {
                        designer.locked = false;
                    }, 1000);

                    var $node = ui.draggable
                                       
                    $node.is(".designer-widget")
                        ? zone.createNode($node.data().widgetType, $node.data().widgetView, index)
                        : zone.addNode($node, index);
                }
            })
            .addClass("content-zone-droppable")
            .data({ dropIndex: index });


        if(designer.modalMode) {
            $dropTarget.html("<p class='cs-zone-text-drop'><i class='fa fa-level-down'></i>&nbsp; Drop Content</p><p class='cs-zone-text-add'><i class='fa fa-plus'></i>&nbsp; Click to Add</p>");

            $dropTarget.on("click", function () {
                designer.openWidgetModal($zone, index)
            });
        }
        else {
            $dropTarget.html("<p>Drop Content</p>");
        }

        return $dropTarget;
    }

    function _createDropTargets() {
        
        $zone.prepend(_makeDroppable(0));

        var dropIndex = 1;

        $zone.children(".content-node").each(function () {
            $(this).after(_makeDroppable(dropIndex));
            dropIndex++;
        });
    }

    function _destroyDropTargets() {
        $zone.children(".content-zone-droppable").remove();
    }

    var zone = {
        init: function () {
            zone.refreshSize();

            if (!$zone.is(".content-zone-embedded")) {
                _destroyDropTargets();
                _createDropTargets();
            }            
        },
        deactivate: function () {
            try {              
                _destroyDropTargets();
            }
            catch (err) { /**/ };
        },
        getMaxWidth: function () {
            return parseFloat($zone.outerWidth());
        },
        getParentNode: function(){
            return $zone.parents(".content-node:first");
        },
        getParentTree: function(){
            return $zone.parents(".content-tree:first");
        },
        getContext: function(){
            var data = $zone.data();
            return {
                name: $zone.attr("name"),
                embedded: data.embedded,
                allowContainers: data.allowContainers,
                allowPadding: data.allowPadding
            }
        },
        indexNodes: function () {
            $zone.children(".content-node").each(function (index) {
                $(this).contentNode().setIndex(index);
                $(this).css({ top: "auto", left: "auto" });
            });

            _destroyDropTargets();
            _createDropTargets();
            zone.refreshSize();
        },
        addNode: function($node, index){
            var $sibling = $zone.children(".content-node:eq(" + index + ")"),
                $oldZone = $node.contentNode().getZone(),
                $widget = $node.find(".content-widget:first"),
                context = zone.getContext();

            if ($sibling.length === 0) {
                $zone.append($node.detach());
            }
            else if ($sibling[0] !== $node[0]) {
                $sibling.before($node.detach());
            }
            //else return;

            // re-index sibling nodes
            $zone.contentZone().indexNodes();
            
            if ($oldZone.length && $zone[0] !== $oldZone[0]) {
                $oldZone.contentZone().indexNodes();
            }

            // cleanup jquery.ui positioning 
            $node.css({ left: "", top: "", width: "" });

            // cleanup ui styles not compatible with this zone
            if (!context.allowContainers) {
                $widget.removeClass("container");
            }

            if (!context.allowPadding) {
                $node.css({ paddingTop: "", paddingBottom: "" });
            }

            // re-initilize the node & childnodes since was detached & reattached to dom
            $node.contentNode().init();

            $node.find(".content-zone").each(function () {
                $(this).contentZone().init();
            });

            $node.find(".content-node").each(function () {
                $(this).contentNode().init();
            });
        },
        createNode: function (type, view, index) {
            var $parent = zone.getParentNode(),
                $tree = zone.getParentTree();

            var postData = {
                tree: $tree.attr("id"),
                parent: $parent.attr("id"),
                zone: zone.getContext(),
                index: index,
                type: type,
                view: view,
            };

            return $zone.load("/sys/content/nodes", postData, "append")
                .done(function (node) {
                    var $node = $("[id='" + $(node).attr("id") + "']");
                    var config = $node.data();
                    zone.addNode($node, index);

                    if (designer.showSettingsOnDrop && config.widgetHasSettings && !config.widgetHasEditor)
                        $node.contentNode().showSettings();               
                });
        },
        refreshSize: function () {
            $zone.parents(".content-cell-group").each(function () {
                var height = $(this).outerHeight() + "px";

                $(this).children().css("min-height", height);
            });
        }
    }

    return zone;
}

$.fn.contentNode = function () {
    var $node = this,
        $widget = this.children(".content-widget:first");

    function _scaleHeight() {
        $node.find("[data-scale-height]").each(function () {
            var scale = +$(this).data().scaleHeight;
            this.style.height = (+$(this).outerWidth() * scale) + "px";
        });
    }

    function _initResizable() {
        var padding = parseFloat($node.css("padding-left")) + parseFloat($node.css("padding-right")),
            rowWidth = node.getZone().contentZone().getMaxWidth(),
            colWidth = rowWidth / 12;

        $widget.resizable({
            grid: colWidth,
            maxWidth: rowWidth,
            resize: function (event, ui) {
                var width = $(this).outerWidth() + padding,
                    size = Math.round(width / colWidth),
                    zone = node.getZone().contentZone();

                size = Math.max(Math.min(size, 12), 1);
                
                this.style.minHeight = this.style.height;
                this.style.height = "";
                this.style.width = "";

                node.setSize(size);
                zone.refreshSize();
                _scaleHeight();
              
            },
            stop: function (event, ui) {
                node.update();
            }
        });
    }

    function _initDraggable() {
        $node.draggable({
            opacity: 0.35,
            distance: 10,
            revert: "invalid",
            cancel: ".k-editor,.wysi-control,.form-control",
            classes: {
                "ui-draggable-dragging": "dragging"
            },
            start: function () {             
                $(".content-node").not($node).children(".content-menu").addClass("hidden")
                designer.processDraggingStart();
            },
            stop: function () {
                $(".content-node").children(".content-menu").removeClass("hidden")
                designer.processDraggingEnd();
            }
        });
    }

    function _initNodeMenu() {
        var $menu = $($(".content-menu-template").html()).prependTo($node),
            settings = node.getZone().contentZone().getContext();

        $menu.css({ marginLeft: -parseFloat($node.css("padding-left")) + "px" });
        $menu.find("a[name=remove]").on("click", node.remove);
        $menu.find("a[name=settings]").on("click", node.showSettings);
        $menu.find("a[name=reload]").on("click", node.reload);
        $menu.find("a[name=background]").on("click", function(){
            node.setBgClass($(this).data().backgroundClass);  
        });
        $menu.find("a[name=classes]").on("click", function () {
            $menu.find("input[name=classes]").val($node.data().nodeClasses);
        });
        $menu.find("input[name=classes]").on("change", function () {
            node.setClasses($(this).val().trim());
        });

        //maxheight
        $menu.find(".maxHeightDropDown").on('show.bs.dropdown', function () {
            $menu.find("input[name=maxheight]").focus()
        })
        $menu.find("a[name=maxheight]").on("click", function () {
            $menu.find("input[name=maxheight]").val($node.data().nodeClasses);
        });
        $menu.find("input[name=maxheight]").on("change", function () {
            node.setMaxHeight($(this).val().trim());
        });
        $menu.find("input[name=maxheight]").val($menu.parent().data().maxHeight);

        //Alignment
        $menu.find("a[name=alignment]").on("click", function () {
            node.setAlignment($(this).data().value);
        });

        // optional padding
        if (settings.allowPadding) {
            $menu.find("li[name=padding]").show();
            $menu.find("a[name=padding]").on("click", function () {
                var value = $(this).data().value;
                node.setPadding(value, value);
            });
        }

        // optional container toggle
        if (settings.allowContainers) {
            $menu.find("li[name=stretch]").show();
            $menu.find("a[name=stretch]").on("click", _toggleFullWidth);
            if (!$widget.is(".container"))
                $menu.find("a[name=stretch] > i").replaceClass("fa-expand", "fa-compress");
        }

        $menu.on("click", function (event) {
            if ($(event.target).parents(".content-menu-item:first").length === 0) {
                $(".content-node").not($node).removeClass("active");
                $node.is(".active") ? $node.removeClass("active") : $node.addClass("active");
            }
        });

        $node.children().on("mouseover", function (event) {
            if (!$(document.body).is(".dragging")) {
                var $related = $(event.target).parents(".content-node:first");

                if ($node[0] === $related[0]) {
                    $node.parents(".content-node").removeClass("hover");
                    $node.addClass("hover");

                    if (!$menu.data().loaded) {
                        _loadCustomMenuItems();
                        $menu.data({ loaded: true });
                    }
                }
            }
        });

        $node.on("mouseleave", function () {
            $node.removeClass("hover");
        });
    }

    function _toggleFullWidth() {             
        var fill = $widget.is(".container");

        return $.post("/sys/content/nodes/" + $node[0].id + "/width", {
            fullWidth: fill
        })
        .done(function () {
            var $icon = $node.children(".content-menu").find("a[name=stretch] > i");

            if (fill) {
                $widget.removeClass("container");
                $icon.replaceClass("fa-expand", "fa-compress");
            }
            else {
                $widget.addClass("container");
                $icon.replaceClass("fa-compress", "fa-expand");
            }
        })
    }

    function _loadCustomMenuItems() {
        var widgetType = $node.data().widgetType,
            widgetId = $node.data().widgetId,
            $menu = $node.children(".content-menu"),
            $end = $menu.find(".content-menu-end");

        if (!widgetId) return;

        var apiUrl = "/sys/content/" + widgetType + "/menu/" + widgetId;

        $.ajax(apiUrl + "?ru=" + encodeURIComponent($.location.fullpath))
           .done(function (menuItems) {
               $menu.find(".content-menu-item.custom").remove();

               menuItems.forEach(function (item) {
                   var $i =  $("<i/>").addClass(item.Icon).addClass("fa-fw"),
                       $a = $("<a/>").attr({ href: item.Url, title: item.Title }).append($i),
                       $li = $("<li/>").addClass("content-menu-item custom").append($a);

                   $end.before($li);
               });
           })
           .fail(function () {
               $itemGroup.append('<li class="text-danger">!err</div>');
           });
    }

    var node = {
        init: function () {
            $node.children(".content-menu").remove();

            if ($node.is(".draggable:not(.ui-draggable)")) _initDraggable();
            //if ($node.is(".resizable:not(.ui-resizable)")) _initResizable();

            _scaleHeight();
            _initNodeMenu();
        },
        deactivate: function () {
            try {
                $node.children(".content-menu").remove();

                if ($node.is(".ui-draggable")) $node.draggable("destroy");
                if ($widget.is(".ui-resizable")) $widget.resizable("destroy");
            }
            catch (err) { /**/ }
        },
        getIndex: function(){
            return +($node[0].dataset.zoneIndex || 0);
        },
        setIndex: function(index){
            $node[0].dataset.zoneIndex = index;
            node.update();            
        },
        getZone: function(){
            return $node.parents(".content-zone:first");
        },
        hasParent: function () {
            return node.getParent().length !== 0;
        },
        getParent: function () {
            return $node.parents(".content-node:first");
        },
        getDimensions: function(){
            return {
                height: $node.outerHeight(),
                width: $node.outerWidth()
            };
        },
        getSize: function(){
            var css = $node[0].classList;
            for (var i = 0; i < css.length; i++) {
                if (/col\-sm\-/.test(css[i])) {
                    return css[i].replace("col-sm-", "");
                }
            }
        },
        setSize: function (size) {
            var css = $node[0].classList;
            for (var i = 0; i < css.length; i++) {
                if (/col\-sm\-/.test(css[i])) {
                    $node.removeClass(css[i]);
                    break;
                }
            }
            $node.addClass("col-sm-" + size);
            node.update();
        },
        setClasses: function(classNames){
            return $.post("/sys/content/nodes/" + $node[0].id + "/css", {
                value: classNames
            })
            .done(function () {
                var prevClassNames = $node.data().nodeClasses;

                $node.removeClass(prevClassNames);               
                $node.addClass(classNames);
                $node.data({ nodeClasses: classNames });
            });
        },
        setMaxHeight: function (maxheight) {
            return $.post("/sys/content/nodes/" + $node[0].id + "/maxheight", {
                    value: maxheight
                })
                .done(function () {
                    var $container = $node.find(".content-widget");
                    $container.css({ maxHeight: maxheight });
                    $container.css({ "overflow-y": "scroll" });
                    $container.css({ "overflow-x": "hidden" });
                });
        },
        setBgClass: function (className) {
            return $.post("/sys/content/nodes/" + $node[0].id + "/bg", {
                value: className
            })
            .done(function () {
                $node.removeClassRegEx(/bg\-/);
                $node.addClass(className);
            });
        },
        setAlignment: function (align) {
            return $.post("/sys/content/nodes/" + $node[0].id + "/alignment", {
                value: align
                })
                .done(function () {
                    var $container = $node.find(".content-widget");
                    $container.css({
                        'position': '',
                        'display': '',
                        'right': '',
                        'transform': '',
                    });
                    if (align === "left") {
                        $container.css({ 'position': "relative" });
                        $container.css({ 'display': "inline-block" });
                        $container.css({ 'right': "0%" });
                        $container.css({ 'transform': "translate(0%)" });
                       
                    }else if (align === "right") {
                        $container.css({ 'position': "relative" });
                        $container.css({ 'display': "inline-block" });
                        $container.css({ 'right': "-100%" });
                        $container.css({ 'transform': "translate(-100%)" });
                    } else if (align === "center") {
                        $container.css({ 'position': "relative" });
                        $container.css({ 'display': "inline-block" });
                        $container.css({ 'right': "-50%" });
                        $container.css({ 'transform': "translate(-50%)" });
                    }
                    
                });
        },
        setPadding: function (top, bottom){
            return $.post("/sys/content/nodes/" + $node[0].id + "/padding", {
                top: top,
                bottom: bottom
            })
            .done(function () {
                $node.css({ paddingTop: top, paddingBottom: bottom });
            });
        },
        update: function () {
            var id = $node.attr("id"),
                parent = node.getParent().attr("id"),
                zone = node.getZone().attr("name"),
                index = node.getIndex();

            return $.post("/sys/content/nodes/" + id, { parent: parent, zone: zone, index: index });
        },
        remove: function() {
            var $zone = node.getZone(),
                widgetId = $node.data('widget-id');

            $.trigger.call($node[0], widgetId + '.destroy', { widgetId: widgetId }); // WJC: widget cleanup.

            $.ajax({ url: "/sys/content/nodes/" + $node.attr("id"), type: "DELETE" }).done(function () {
                $node.remove();

                $zone.contentZone().indexNodes();
            });
        },
        reload: function () {
            var id = $node.attr("id"),
                zone = node.getZone().contentZone(),
                zoneContext = zone.getContext(),
                widgetId = $node.data('widget-id');

            $.trigger.call($node[0], widgetId + '.destroy', { widgetId: widgetId }); // WJC: widget cleanup.

            return $node.load("/sys/content/nodes/" + id, { zone: zoneContext }, "replace").done(function () {
                // reset internal references to newly created dom objects
                $node = $("[id='" + id + "']");
                $widget = $node.children(".content-widget:first");
  
                // re-attach events
                node.init();
                designer.init(id, designer.modalMode);
                $.trigger.call($node[0], widgetId + '.loaded', { widgetId: widgetId }); // WJC: init widget.
                zone.refreshSize();
            });
        },
        showHelp: function () {
            alert($node.attr("id"));
        },
        showSettings: function(){
            var params = {
                type: $node.data().widgetType,
                id: $node.data().widgetId
            }

            $.dialog("/Components/WidgetForm", params).done(function (button) {
                if (button === "save") {
                    var $modal = $(this), promises = [];

                    $modal.find("form").each(function(){
                        promises.push($(this).form().post());
                    })

                    return $.when.apply($, promises).done(function () {
                        node.reload();
                    });
                }
            });
        },       
    }

    return node;
}

$.fn.designerWidget = function () {
    var $widget = this;

    function _initDraggable() {
        $widget.draggable({
            opacity: 0.5,
            revert: "invalid",
            appendTo: "body",
            classes: {
                "ui-draggable": "",
                "ui-draggable-handle": "",
                "ui-draggable-dragging": "dragging"
            },
            helper: function (e) {
                var $helper = $(this).clone(),
                    dy = $helper.outerHeight(),
                    dx = $helper.outerWidth() / 2;

                $helper.css({
                    position: "absolute",
                    zIndex: 1000,
                    left: (e.pageX - dx) + "px",
                    top: (e.pageY - dy) + "px"
                })
                .addClass("designer-widget-clone");

                $(document.body).append($helper);

                return $helper;
            },
            start: function () {
                $(".content-node").children(".content-menu").addClass("hidden");
                designer.processDraggingStart();
            },
            stop: function () {
                $(".content-node").children(".content-menu").removeClass("hidden");
                designer.processDraggingEnd();
            }
        });
    }

    var widget = {
        init: function () {
            if (!$widget.is(".ui-draggable")) _initDraggable();
        },
        deactivate: function(){
            if ($widget.is(".ui-draggable")) $widget.draggable("destroy");
        }
    }

    return widget;
}

//----------------------------------------------------------------
// Designer Object
//----------------------------------------------------------------
var designer = {
    visible: true,
    mode: "bar",
    showSettingsOnDrop: true,
    modalMode: false,
    locked: false,
    init: function (treeId, modalMode) {
        var $body = $(document.body);
        var $tree = $("#" + treeId);

        designer.modalMode = !!modalMode;

        // initialize components
        $tree.find(".content-zone.editable").each(function () {
            $(this).contentZone().init();
        });

        $tree.find(".content-node.editable").each(function () {
            $(this).contentNode().init();
        });

        // attach panel events
        if (!modalMode) {
            $body.addClass("design-mode designer-bar-active");
            
            $(".designer-bar").find(".designer-widget").each(function () {
                $(this).designerWidget().init();
            });

            $(".designer-bar-body").css("background-color", $body.css("background-color"));

            $(".designer-bar-tabs a").on("click", function () {
                var categoryId = $(this).data().categoryId;

                $(".designer-bar-tabs a").removeClass("active").filterByData({ categoryId: categoryId }).addClass("active");
                $(".designer-bar-views span").removeClass("visible").hide();
                $(".designer-bar-views span").filterByData({ categoryId: categoryId }).addClass("visible").show();
            })

            $(".designer-bar-buttons a[name=forms]").on("click", function () {
                var $icon = $(this).find("i.fa");

                $icon.toggleClass("fa-check-square-o");
                $icon.toggleClass("fa-square-o");

                this.showSettingsOnDrop = !this.showSettingsOnDrop;
            });

            $(".designer-bar-toggle a").on("click", function () {
                $(document.body).is(".designer-bar-active") ? designer.hideDesignBar() : designer.showDesignBar();
            });
        }

    },

    processDraggingStart: function () {
        $(document.body).addClass("dragging");
        
        if (!designer.modalMode)
            designer.hideDesignBar();
    },

    processDraggingEnd: function () {
        $(document.body).removeClass("dragging");

        if (!designer.modalMode)
            designer.showDesignBar();
    },

    showDesignBar: function () {    
        $(document.body).addClass("designer-bar-active")
        $('.designer-bar-toggle i').attr("class", "fa fa-chevron-down");
    },
    hideDesignBar: function(){                         
        $(document.body).removeClass("designer-bar-active")
        $('.designer-bar-toggle i').attr("class", "fa fa-chevron-up");
    },
    openWidgetModal: function ($zone, index) {
        var content = $(".cs-designer-modal-template").html();
        var $modal = $("<div></div>").addClass("modal");

        $modal.append(content);
        $modal.appendTo($(document.body));
        $modal.show();

        $modal.data({ targetZone: $zone, targetIndex: index });

        // initialize panel events
        $modal.find(".cs-designer-tabs a").on("click", function () {
            var category = $(this).data().category;

            // set the tab strip state
            $modal.find(".cs-designer-category.active").removeClass("active");
            $modal.find(".cs-designer-category").filterByData({ category: category }).addClass("active")

            // show / hide widgets for that category
            $modal.find(".cs-designer-widget").addClass("cs-hide");
            $modal.find(".cs-designer-widget").filterByData({ category: category }).removeClass("cs-hide");
        })

        $modal.find(".modal-close").on("click", function () {
            $modal.remove();
        });

        // initialize widget events
        $modal.find(".cs-designer-widget a").on("click", function () {
            var $widget = $(this).parent();
            var zone = $zone.contentZone();

            zone.createNode($widget.data().widgetType, $widget.data().widgetView, index);
            $modal.remove();
        })
    },
    closeWidgetModal: function () {
        $(".cs-designer-modal").parent().remove();
    }
}

