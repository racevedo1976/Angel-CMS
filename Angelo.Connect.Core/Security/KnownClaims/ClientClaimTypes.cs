using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security.KnownClaims
{
    public static class ClientClaimTypes
    {
        public const string PrimaryAdmin = "client-primary-admin";

        public const string UserDirectoryCreate = "client-dir-create";
        public const string UserDirectoryRead = "client-dir-read";
        public const string UserDirectoryEdit = "client-dir-edit";
        public const string UserDirectoryDelete = "client-dir-delete";

        public const string UsersCreate = "client-users-create";
        public const string UsersRead = "client-users-read";
        public const string UsersEdit = "client-users-edit";
        public const string UsersDelete = "client-users-delete";
        public const string UsersStatusEdit = "client-users-status";

        public const string AppRolesCreate = "client-roles-create";
        public const string AppRolesRead = "client-roles-read";
        public const string AppRolesEdit = "client-roles-edit";
        public const string AppRolesDelete = "client-roles-delete";
        public const string AppRolesAssign = "client-roles-map";


        public const string AppGroupsCreate = "client-groups-create";
        public const string AppGroupsRead = "client-groups-read";
        public const string AppGroupsEdit = "client-groups-edit";
        public const string AppGroupsDelete = "client-groups-delete";
        public const string AppGroupsAssign = "client-groups-map";

        public const string AppNotifyGroupsCreate = "client-notegroups-create";
        public const string AppNotifyGroupsRead = "client-notegroups-read";
        public const string AppNotifyGroupsEdit = "client-notegroups-edit";
        public const string AppNotifyGroupsDelete = "client-notegroups-delete";
        public const string AppNotifyGroupsAssign = "client-notegroups-map";
        public const string AppNotificationsSend = "client-notifications-send";

        public const string AppLibraryRead = "client-library-read";
        public const string AppLibraryOwner = "client-library-owner";
        
        public const string SiteCollectionsCreate = "client-sitecollections-create";
        public const string SiteCollectionsRead = "client-sitecollections-read";
        public const string SiteCollectionsEdit = "client-sitecollections-edit";
        public const string SiteCollectionsDelete = "client-sitecollections-delete";
        public const string SiteCollectionsAssign = "client-sitecollections-assign";

        public const string SitesCreate = "client-sites-create";
        public const string SitesRead = "client-sites-read";
        public const string SitesEdit = "client-sites-edit";
        public const string SitesDelete = "client-sites-delete";
        public const string SitesPublish = "client-sites-publish";

        public const string UserLibrariesBrowse = "client-user-library-browse";
        public const string UserLibrariesManage = "client-user-library-manage";

        public const string UserContentBrowse = "client-user-content-browse";
        public const string UserContentManage = "client-user-content-manage";
        public const string UserContentPublish = "client-user-content-publish";

    }
}
