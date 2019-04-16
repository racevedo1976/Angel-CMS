var page = function () {

    var _submitAsForm = function (action, method, data) {
        var form = document.createElement("form"), field;

        form.action = action;
        form.method = method || "post";
        
        if (typeof (data) === "object") {
            for (var key in data) {
                field = document.createElement("input");
                field.name = key;
                field.value = data[key]            
                form.appendChild(element1);
            }
        }

        document.body.appendChild(form);
        form.submit();
    }

    return {
        postAsForm: function(url){
            _submitAsForm(url);
        },
        openWindow: function(url)
        {
            window.open(url, "_blank").focus();
        },
        reload: function(){
            location.reload();
        },
        resizeAdminMenu: function () {
            var $adminMenu = $(".sys-admin-menu"),
                $userToolbar = $(".sys-user-toolbar"),
                topMenuOffset = $userToolbar.offset(),
                topMenuHeight = $userToolbar.outerHeight(true),
                pageHeight = window.screen.height;

            $adminMenu
                .offset({
                    /*left: topMenuOffset.left,*/
                    top: topMenuOffset.top + topMenuHeight
                })
                .css({
                    height: pageHeight - topMenuHeight + "px"
                });

            return $adminMenu;
        },
        toggleAdminMenu: function () {
            var $adminMenu = $(".sys-admin-menu");

            $adminMenu.resize = this.resizeAdminMenu;

            if ($adminMenu.is(".slide-right-in")) {
                $adminMenu.removeClass("slide-right-in").addClass("slide-left-out");
            }
            else {
                $adminMenu.show().resize().removeClass("slide-left-out").addClass("slide-right-in");
            }

            return $adminMenu;
        },
    }
}();