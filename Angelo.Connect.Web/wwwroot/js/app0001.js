requirejs.config({
    paths: {
        "designer": "/js/designer",
        "widgets": "/js/widgets",
        "modules": "/js/modules",
        "console": "/js/console",
        "resources": "/js/resources",
        "site": "/js/modules/site",
        "kendo": "/js/kendo-ui/kendo.all.min",
    },
    shim: {
        'designer': { exports: 'designer' },
    },
    urlArgs: function () {
        return "?v=" + +new Date;
    }
});

define([], function () {
    console.log("RequireJs Modules Init");
});