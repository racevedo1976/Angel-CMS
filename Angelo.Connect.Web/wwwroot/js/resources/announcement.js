define(["resources/common"], function(common){

    var resources = {
        announcementCategoryCreatePrompt: "Enter a unique category name",
        announcementCategoryConfirmDelete: "Are you sure you want to delete this Category?",
        announcementCategoryDeleted: "This category has been deleted",
        announcementPostConfirmDelete: "Are you sure you want to delete all versions of this post?",
        announcementPostDeleted: "This announcement post has been deleted."
    };

    // merge announcement resources with common resources into new empty object
    return $.extend({}, common, resources);
});