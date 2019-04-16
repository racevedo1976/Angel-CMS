using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security.KnownClaims
{
    public class SiteClaimTypes
    {
        public const string SitePrimaryAdmin = "site-primary-admin";

        public const string SiteSettingsRead = "site-settings-read";
        public const string SiteSettingsEdit = "site-settings-edit";

        public const string SiteTemplateRead = "site-templates-read";
        public const string SiteTemplateEdit = "site-templates-edit";

        public const string SiteUsersCreate = "site-users-create";
        public const string SiteUsersRead = "site-users-read";
        public const string SiteUsersEdit = "site-users-edit";
        //public const string SiteUsersDelete = "site-users-delete";

        public const string SiteRolesCreate = "site-roles-create";
        public const string SiteRolesRead = "site-roles-read";
        public const string SiteRolesEdit = "site-roles-edit";
        public const string SiteRolesDelete = "site-roles-delete";
        public const string SiteRolesAssign = "site-roles-map";

        public const string SiteGroupsCreate = "site-groups-create";
        public const string SiteGroupsRead = "site-groups-read";
        public const string SiteGroupsEdit = "site-groups-edit";
        public const string SiteGroupsDelete = "site-groups-delete";
        public const string SiteGroupsAssign = "site-groups-map";

        public const string SiteNotifyGroupsCreate = "site-notegroups-create";
        public const string SiteNotifyGroupsRead = "site-notegroups-read";
        public const string SiteNotifyGroupsEdit = "site-notegroups-edit";
        public const string SiteNotifyGroupsDelete = "site-notegroups-delete";
        public const string SiteNotifyGroupsAssign = "site-notegroups-map";
        public const string SiteNotificationsSend = "site-notifications-send";

        public const string SiteMasterPagesCreate = "site-masterpages-create";
        public const string SiteMasterPagesRead = "site-masterpages-read";
        public const string SiteMasterPagesEdit = "site-masterpages-edit";
        public const string SiteMasterPagesDelete = "site-masterpages-delete";
        public const string SiteMasterPagesDesign = "site-masterpages-design";
        public const string SiteMasterPagesPublish = "site-masterpages-publish";

        public const string SitePagesCreate = "site-pages-create";
        public const string SitePagesRead = "site-pages-read";
        public const string SitePagesEdit = "site-pages-edit";
        public const string SitePagesDelete = "site-pages-delete";
        public const string SitePagesDesign = "site-pages-design";
        public const string SitePagesPublish = "site-pages-publish";

        public const string SiteLibraryOwner = "site-library-owner";
        public const string SiteLibraryReader = "site-library-reader";
        //public const string SiteLibraryEditFolders = "site-library-folders-edit";
        //public const string SiteLibraryDeleteFolders = "site-library-folders-delete";
        //public const string SiteLibraryUploadFiles = "site-library-files-upload";
        //public const string SiteLibraryEditFiles = "site-library-files-edit";
        //public const string SiteLibraryDeleteFiles = "site-library-files-delete";
        //public const string SiteLibraryManageSecurity = "site-library-security";

        public const string SiteNavMenusCreate = "site-menus-create";
        public const string SiteNavMenusRead = "site-menus-read";
        public const string SiteNavMenusEdit = "site-menus-edit";
        public const string SiteNavMenusDelete = "site-menus-delete";
    }
}
