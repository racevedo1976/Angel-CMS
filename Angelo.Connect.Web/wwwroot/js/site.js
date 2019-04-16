// search autocomplete init
$(document).ready(function () {
    
    var validTags = ["a","abbr","address","area","article","aside","audio","b","base","bdi","bdo","blockquote","body","br","button","canvas","caption","cite","code","col","colgroup","data","datalist","dd","del","dfn","div","dl","dt","em","embed","fieldset","figcaption","figure","footer","form","h1 - h6","head","header","hr","html","i","iframe","img","input","ins","kbd","keygen","label","legend","li","link","main","map","mark","meta","meter","nav","noscript","object","ol","optgroup","option","output","p","param","pre","progress","q","rb","rp","rt","rtc","ruby","s","samp","script","section","select","small","source","span","strong","style","sub","sup","table","tbody","td","template","textarea","tfoot","th","thead","time","title","tr","track","u","ul","var","video","wbr"];

    $("input[data-autocomplete-url]").each(function () {
        var $el = $(this);
        var data = $el.data();

        $('#autocomplete').autocomplete({
            serviceUrl: data.autocompleteUrl,
            lookupLimit: data.autocompleteLookuplimit,
            minChars: data.autocompleteMinchars,
            maxHeight: data.autocompleteMaxheight,
            deferRequestBy: data.autocompleteDeferRequestBy,
            width: data.autocompleteWidth,
            showNoSuggestionNotice: data.autocompleteShowNoSuggestionNotice,
            suggestionNotice: data.autocompleteSuggestionNotice,
            groupBy: data.autocompleteGroupBy,
            onSelect: function (suggestion) {
                window.location.href = data.autocompleteAction + '?id=' + suggestion.data;
            }
        });
    });
    
    $(document).on("click", "[data-expand]", function () {
        var target = $(this).data().expand,
            $target = tryEvalSelector.call(this, target);

        if (isJQueryObject($target)) {
            $target.slideToggle(200);
        }
    });
    
    $(document).on("click", "[data-toggle-css]", function() {
        var data = tryEval($(this).data().toggleCss);

        if ($.isArray(data) && data.length) {
            var target = data.length > 2 ? data.shift() : "",
                $target = tryEvalSelector.call(this, target);

            if (isJQueryObject($target)) {
                $target.replaceClass(data[0], data.length > 1 ? data[1] : "");
            }
        }
    });
    
    $(document).on("click", "[data-activate]", function() {
        var target = $(this).data().activate,
            $target = tryEvalSelector.call(this, target);

        if (isJQueryObject($target)) {
            $target.addClass("active").siblings().removeClass("active");
        }
    });

    $(document).on("click", "[data-activate-parent]", function () {
        var target = $(this).data().activateParent,
            $target = $(this).parents(target);

        if (isJQueryObject($target)) {
            $target.first().addClass("active").siblings().removeClass("active");
        }
    });

    $(document).on("click", "[data-activate-child]", function () {
        var target = $(this).data().activateChild,
            $target = $(this).find(target);

        if (isJQueryObject($target)) {
            $target.first().addClass("active").siblings().removeClass("active");
        }
    });

    $('#list').click(function (event) { event.preventDefault(); $('#items .item').addClass('list-group-item'); });
    $('#grid').click(function (event) { event.preventDefault(); $('#items .item').removeClass('list-group-item'); $('#items .item').addClass('grid-group-item'); });

    function tryEvalSelector(target) {    
        if (typeof target === "object" && target.tagName) {
            target = $(target);
        }
        else if (typeof target === "string" && target.length) {
            target = target[0] === "$" ? tryEval.call(this, target) : $(target);
        }
        else if (typeof this === "object" && this.tagName) {
            target = $(this)
        }
        return target;
    }

    function isJQueryObject(obj) {
        return typeof obj === "object" && (obj instanceof jQuery || 'jquery' in Object(obj));
    }
    
    function tryEval(expression) {
        try {
            return eval(expression);
        }
        catch (err) {
            console.error("Could not evaluate: " + expression);
        }
    }
})


$.on = function (eventName, callback) {
    var source = this;
    return $(document).on(eventName, function (event, data, source) {
        return callback.call(source || document, event, data);
    });
}

$.one = function (eventName, callback) {
    var source = this;
    return $(document).one(eventName, function (event, data) {
        return callback.call(source || document, event, data);
    });
}

$.off = function (eventName) {
    // usage $.off("grid.select")
    if (eventName) {
        $(document).off(eventName);
        return $;
    }
    
    // usage $.off().on("grid.select")
    return {
        on: function (eventName, callback) {
            return $.off(eventName).on(eventName, callback);
        }
    }
}

$.trigger = function (eventName, data) {
    var source = this;

    return $(document).trigger(eventName, data, source);
}

$.fn.filterByData = function (data) {
    if (!$.isPlainObject(data))
        return $();

    return this.filter(function () {
        var tdata = $(this).data() || {};
        for (var key in data) {
            if (tdata[key] !== data[key])
                return false;
        }
        return true;
    });
}

$.load = function (url, data) {
    return $(document.body).load(url, data, "append");
}

$.fn.load = function (url, data, behavior) {
    var task = $.Deferred(),
        $container = this,
        $result = $();

    $.ajax({
        url: url,
        data: $.extend({}, data, { ts: +new Date }), // cache buster
        dataType: "html",       
        success: function (result) {
            if (behavior === "replace") {
                $container.before(result);
                $result = $container.prev();
                $container.remove();
                $container = $result.parent();
            }
            else if (behavior === "append") {
                $container.append(result);
                $result = $container.children(":last");
            }
            else if (behavior === "prepend") {
                $container.prepend(result);
                $result = $container.children(":first");
            }
            else {
                $container.html(result);
                $result = $container.children(":first");
            }

            $.trigger.call($container[0], "dom.change", { container: $container[0], result: $result[0] });

            task.resolve($result);
        },
        error: task.reject
    });

    return task.promise();
};

$.fn.loadComponent = function (settings) {
    if (typeof settings !== "object" || !settings.type)
        throw new Error("Component settings should be { type: <string>, id: <string>, params: <object> }");

    settings.id = settings.id || settings.type;

    return $("<component></component>").appendTo(this.empty())
        .attr("id", settings.id + "_component")
        .data({ component: settings })
        .load("/components/" + settings.type + "/" + settings.id, settings.params)
        .done(function () {
            $.trigger(settings.id + ".load", settings);
        });
}

$.fn.component = function () {
    var $component = this.parents("component:first");

    if (!$component.length) {
        $.trigger("component.error", this.attr("id"));
        return;
    }

    var data = $component.data().component,
        url = "/components/" + data.type + "/" + data.id;

    function onload() {
        $.trigger(data.id + ".load", data, $component[0], this);
    }

    var methods = {
        invoke: function (params) {
            data.params = params;
            return $component.load(url, params).done(onload);
        },
        reload: function () {
            return $component.load(url, $.extend({}, data.params)).done(onload);
        }
    }

    return $.extend({}, methods, data);
}

$.component = function (id) {
    return $("#" + id.replace("#", '')).component();
}

$.fn.tabs = function () {
    var $tabs = $(this);

    function _findTab(name) {
        return +name === name
            ? $tabs.find('li:eq(' + name + ') a')
            : $tabs.find('a[href="#' + name + '"]');
    }

    function _disable(name) {
        _findTab(name).attr("disabled", "disabled").parent().removeClass("active").addClass("disabled");
    }

    function _enable(name) {
        _findTab(name).removeAttr("disabled").parent().removeClass("disabled");
    }
  
    return {       
        show: function (name) {
            _enable(name);
            _findTab(name).addClass("active").tab("show");
        },
        disable: _disable,
        enable: _enable
    }
}

$.isPromise = function (result) {
    // good enough solution for testing for jQuery promises
    return typeof result === "object" && typeof result.then === "function" && typeof result.done === "function";
}

// extended location object
$.location =  $.extend({}, location, {
    fullpath: location.href.replace(/^(?:\/\/|[^\/]+)*\//, "/")
});

// jQuery Css Extensions
// TODO: the custom toggleClass overwrites jquery default behavior. use .replaceClass instead
$.fn.replaceClass = function (class1, class2) {
    var $el = $(this);

    if ($el.hasClass(class1))
        $el.removeClass(class1).addClass(class2);
    else if ($el.hasClass(class2))
        $el.removeClass(class2).addClass(class1);
    else
        $el.addClass(class1);

    return $el;
}

//$.fn.replaceClass = $.fn.toggleClass;

$.fn.removeClassRegEx = function (regex) {
    return this.each(function () {
        var classes = $(this).attr('class');
        if (!classes || !regex) return false;

        var unmatched = [];
        classes = classes.split(' ');

        for (var i = 0; i < classes.length; i++)
            if (!classes[i].match(regex))
                unmatched.push(classes[i]);

        $(this).attr('class', unmatched.join(' '));
    });
};

// Ajax Routing
$(document).on("click", "a[data-route]", function (event) {
    event.preventDefault();

    var $a = $(this),
        $target = $($a.data().routeHost),
        route = $a.data().route;

    // If no host is specified inline, check to see if it's been specified
    // in a parent element (eg, a menu where all links will target the same host)
    if (!$target.length) {
        $a.parents("[data-route-host]:first").each(function () {
            $target = $($(this).data().routeHost);
        });
    }

    // Otherwise if the link is inside a host, then navigate the parent host (eg, iframe'ish)
    if (!$target.length) {
        $target = $a.parents(".route-host:first");
    }

    // Else throw an error - can't navigate if we don't know where to place the results
    if (!$target.length) {
        return console.error("Missing route host for route " + route);
    }

    return $.ajax({
        url: route,
        dataType: "html",
        data: { v: +new Date }, // cache buster
        success: function (html) {
            $target.addClass("route-host");
            $target.html(html);
            $.trigger.call($target[0], "dom.change", { container: $target[0] });
        }
    });
});

// RequireJs Extensions
$.fn.attachModule = function (module, data) {
   var $node = this, promise = $.Deferred();

    require([module], function (module) {
        promise.resolve(module);
    });

    return promise.then(function (module) {

        if (typeof module === "function") {
            module = module.call(window);
        }

        if ($.isPlainObject(module) && typeof module.attach === "function") {
            module.attach($node, data);
        }

        $node.data({
            getModule: function () {
                return module;
            }
        });

        return {
            $node: $node,
            module: module,
            data: data
        };
    });
}

$.fn.attachChildModules = function () {
    var $container = this;

    $container.find("[data-module]").not("a").each(function () {
        var $node = $(this), data = $node.data();

        if (!data.getModule) {
            $node.attachModule(data.module, data);
        }
    })
}

$(document).ready(function () {
    $(document.body).attachChildModules();
});

$.on("dom.change", function (event, data) {
    $(this).attachChildModules();
});

// Non-requirejs way to load the console from menus, etc
$.console = function (component, route) {
    var dialogUrl = "/sys/console/dialog";

    if(component)  
        dialogUrl += "/" + component;

    if(route) 
        dialogUrl += "?route=" + window.encodeURIComponent(route);

    return $.dialog(dialogUrl);
}


// TODO: What is this? Needs to be refactored / removed
function loadDialogModalByRoute(route, title) {
    $.dialog(route)
        .done(function () {
            switch (title) {
                case 'connection_groups':
                    $("#UserId").data("kendoDropDownList").destroy();
                    $("#UserId").remove();
                    break;
            }
        });
}