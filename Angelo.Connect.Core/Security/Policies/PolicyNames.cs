using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security
{
    public class PolicyNames
    {
        // generic policies
        public const string StubPolicy = "AnyUserPolicy";

        // corporate policies
        public const string CorpUser = "IsCorpUser";
        public const string CorpSiteCanManage = "CanManagerCorpSites";
        public const string CorpClientsCreate = "CanCreateClients";
        public const string CorpClientsEdit = "CanEditClients";
        public const string CorpClientsRead = "CanReadClients";
        public const string CorpClientsDelete = "CanDeleteClients";
        public const string CorpLibraryBrowse = "CanBrowseCorpLibrary";
        public const string CorpLibraryCreateFolders = "CanCreateCorpLibraryFolders";
        public const string CorpLibraryDeleteFiles = "CanDeleteCorpLibraryFiles";
        public const string CorpLibraryDeleteFolders = "CanDeleteCorpLibraryFolders";
        public const string CorpLibraryEditFiles = "CanEditCorpLibraryFiles";
        public const string CorpLibraryEditFolders = "CanEditCorpLibraryFolders";
        public const string CorpLibraryManageSecurity = "CanManageCorpLibrarySecurity";
        public const string CorpLibraryUploadFiles = "CanUploadCorpLibraryFiles";
        public const string CorpProductsCreate = "CanCreateProducts";
        public const string CorpProductsEdit = "CanEditProducts";
        public const string CorpProductsRead = "CanReadProducts";
        public const string CorpProductsDelete = "CanDeleteProducts";
        public const string CorpProductsMap = "CanAssignProducts";

        // client policies
        public const string ClientLevelAny = "HasAnyClientLevelClaim";
        public const string ClientRolesCreate = "CanCreateClientRoles";
        public const string ClientRolesRead = "CanReadClientRoles";
        public const string ClientRolesEdit = "CanEditClientRoles";
        public const string ClientRolesDelete = "CanDeleteClientRoles";
        public const string ClientDirectoriesRead = "CanReadClientDirectories";
        public const string ClientDirectoriesEdit = "CanEditClientDirectories";
        public const string ClientDirectoriesDelete = "CanDeleteClientDirectories";
        public const string ClientDirectoriesCreate = "CanCreateClientDirectories";
        public const string ClientUsersCreate = "CanCreateClientUserProfiles";
        public const string ClientUsersRead = "CanReadClientUserProfiles";
        public const string ClientUsersEdit = "CanEditClientUserProfiles";
        public const string ClientSitesRead = "CanReadClientSites";
        public const string ClientSitesDelete = "CanDeleteClientSites";
        public const string ClientSitesEdit = "CanEditClientSites";
        public const string ClientSitesCreate = "CanCreateClientSites";
        public const string ClientGroupsRead = "CanReadClientGroups";
        public const string ClientGroupsEdit = "CanEditClientGroups";
        public const string ClientGroupsDelete = "CanDeleteClientGroups";
        public const string ClientGroupsCreate = "CanCreateClientGroups";
        public const string ClientGroupsAssign = "CanAssignClientGroups";
        public const string ClientNotificationGroupRead = "CanReadClientNotificationGroups";
        public const string ClientNotificationGroupEdit = "CanEditClientNotificationGroups";
        public const string ClientNotificationGroupCreate = "CanCreateClientNotificationGroups";
        public const string ClientNotificationGroupDelete = "CanDeleteClientNotificationGroups";
        public const string ClientLibraryRead = "CanReadClientLibrary";
        public const string ClientLibraryOwner = "CanManageClientLibrary";
        


        // site policies
        public const string SiteAnyClaimToSite = "HasAnyClaimToSite";
        public const string SiteLevelAny = "HasAnySiteLevelClaim";
        public const string SiteUsersCreate = "CanCreateSiteUserProfiles";
        public const string SiteUsersRead = "CanReadSiteUserProfiles";
        public const string SiteUsersEdit = "CanEditSiteUserProfiles";
        public const string SiteRolesCreate = "CanCreateSiteRoles";
        public const string SiteRolesRead = "CanReadSiteRoles";
        public const string SiteRolesEdit = "CanEditSiteRoles";
        public const string SiteRolesDelete = "CanDeleteSiteRoles";
        public const string SitePagesRead = "CanReadSitePages";
        public const string SitePagesEdit = "CanEditSitePages";
        public const string SitePagesCreate = "CanCreateSitePages";
        public const string SitePagesDelete = "CanDeleteSitePages";
        public const string SitePagesPublish = "CanPublishSitePages";
        public const string SitePagesDesign = "CanDesignSitePages";
        public const string SiteSettingsRead = "CanReadSiteSettings";
        public const string SiteSettingsEdit = "CanEditSiteSettings";
        public const string SiteGroupsRead = "CanReadSiteGroups";
        public const string SiteGroupsEdit = "CanEditSiteGroups";
        public const string SiteGroupsCreate = "CanCreateSiteGroups";
        public const string SiteGroupsDelete = "CanDeleteSiteGroups";
        public const string SiteGroupsAssign = "CanAssignSiteGroups";
        public const string SiteNotificationGroupRead = "CanReadSiteNotificationGroups";
        public const string SiteNotificationGroupEdit = "CanEditSiteNotificationGroups";
        public const string SiteNotificationGroupCreate = "CanCreateSiteNotificationGroups";
        public const string SiteNotificationGroupDelete = "CanDeleteSiteNotificationGroups";
        public const string SiteMasterPagesRead = "CanReadSiteMasterPages";
        public const string SiteMasterPagesEdit = "CanEditSiteMasterPages";
        public const string SiteMasterPagesCreate = "CanCreateSiteMasterPages";
        public const string SiteMasterPagesDelete = "CanDeleteSiteMasterPages";
        public const string SiteTemplatesRead = "CanReadSiteTemplates";
        public const string SiteTemplatesEdit = "CanEditSiteTemplates";
        public const string SiteNavMenusRead = "CanReadSiteNavMenus";
        public const string SiteNavMenusEdit = "CanEditSiteNavMenus";
        public const string SiteNavMenusCreate = "CanCreateSiteNavMenus";
        public const string SiteNavMenusDelete = "CanDeleteSiteNavMenus";
        public const string SiteLibraryRead = "CanReadSiteLibrary";
        public const string SiteLibraryOwner = "HasOwnerSiteLibraryClaim";

    }
}
