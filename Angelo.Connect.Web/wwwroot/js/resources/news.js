define(["resources/common"], function(common){

    var resources = {
        newsCategoryCreatePrompt: "Enter a unique category name",
        newsCategoryConfirmDelete: "Are you sure you want to delete this Category?",
        newsCategoryDeleted: "This category has been deleted",
        newsPostConfirmDelete: "Are you sure you want to delete all versions of this post?",
        newsPostDeleted: "This news post has been deleted."
    };

    // merge news resources with common resources into new empty object
    return $.extend({}, common, resources);
});