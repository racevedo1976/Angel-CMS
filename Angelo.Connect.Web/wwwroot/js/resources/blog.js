define(["resources/common"], function(common){

    var resources = {
        blogCategoryCreatePrompt: "Enter a unique category name",
        blogCategoryConfirmDelete: "Are you sure you want to delete this Category?",
        blogCategoryDeleted: "This category has been deleted",
        blogPostConfirmDelete: "Are you sure you want to delete all versions of this post?",
        blogPostDeleted: "This blog post has been deleted."
    };

    // merge blog resources with common resources into new empty object
    return $.extend({}, common, resources);
});