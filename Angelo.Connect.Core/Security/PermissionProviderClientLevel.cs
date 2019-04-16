using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Identity.Models;

namespace Angelo.Connect.Security
{
    public class PermissionProviderClientLevel : ISecurityPermissionProvider
    {
        public PoolType Level { get; } = PoolType.Client;

        private List<Permission> _permissions;

        public PermissionProviderClientLevel(IContextAccessor<ClientAdminContext> clientContextAccessor)
        {
            _permissions = new List<Permission>();

            CreatePermissionGroups(clientContextAccessor.GetContext());
        }
     
        private void CreatePermissionGroups(ClientAdminContext clientContext)
        {
            if (clientContext?.Client == null)
                throw new NullReferenceException("ClientAdminContext.Client is required");

            var currentClientId = clientContext.Client.Id;

            //build permissions groups
            _permissions.Add(new Permission
            {
                Title = "Users",
                Permissions = new Permission[]
                {
                    new Permission {
                        Title = "Browse All User Directories",
                        Claims = {
                            new SecurityClaim(ClientClaimTypes.UserDirectoryRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.UsersRead, currentClientId)
                        }
                    },
                    new Permission {
                        Title = "Manage / Create User Directories",
                        Claims = {
                            new SecurityClaim(ClientClaimTypes.UserDirectoryCreate, currentClientId),
                            new SecurityClaim(ClientClaimTypes.UserDirectoryRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.UserDirectoryEdit, currentClientId),
                            new SecurityClaim(ClientClaimTypes.UserDirectoryDelete, currentClientId),
                        }
                    },                  
                    new Permission {
                        Title = "Create / Edit User Profiles",
                        Claims = {
                            new SecurityClaim(ClientClaimTypes.UserDirectoryRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.UsersRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.UsersCreate, currentClientId),
                            new SecurityClaim(ClientClaimTypes.UsersEdit, currentClientId),
                            new SecurityClaim(ClientClaimTypes.UsersDelete, currentClientId),
                        }
                    },
                    new Permission {
                        Title = "Assign Global Roles to Users",
                        Claims = {
                            new SecurityClaim(ClientClaimTypes.UserDirectoryRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.UsersRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppRolesRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppRolesAssign, currentClientId),
                        }
                    },
                    new Permission {
                        Title = "Assign Site Roles to Users",
                        Claims = {
                            new SecurityClaim(SiteClaimTypes.SiteUsersRead, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteRolesAssign, currentClientId),
                        }
                    },
                }
            });

            _permissions.Add(new Permission
            {
                Title = "Roles",
                Permissions = new Permission[]
                {
                    new Permission {
                        Title = "Manage / Create Global Roles",
                        Claims =
                        {
                            new SecurityClaim(ClientClaimTypes.AppRolesRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppRolesCreate, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppRolesEdit, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppRolesDelete, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppRolesAssign, currentClientId),
                        }
                    },
                    new Permission {
                        Title = "Manage / Create Site Roles",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteRolesRead, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteRolesCreate, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteRolesEdit, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteRolesDelete, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteRolesAssign, currentClientId),
                        }
                    },
                    

                }
            });

            _permissions.Add(new Permission
            {
                Title = "Connection Groups",
                Permissions = new Permission[]
                {
                    new Permission {
                        Title = "Manage Global Connection Groups",
                        Claims =
                        {
                            new SecurityClaim(ClientClaimTypes.AppGroupsRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppGroupsCreate, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppGroupsEdit, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppGroupsDelete, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppGroupsAssign, currentClientId),
                        }
                    },
                    new Permission {
                        Title = "Manage All Site Connection Groups",
                        Claims =
                        {
                             new SecurityClaim(SiteClaimTypes.SiteGroupsRead, currentClientId),
                             new SecurityClaim(SiteClaimTypes.SiteGroupsCreate, currentClientId),
                             new SecurityClaim(SiteClaimTypes.SiteGroupsEdit, currentClientId),
                             new SecurityClaim(SiteClaimTypes.SiteGroupsDelete, currentClientId),
                             new SecurityClaim(SiteClaimTypes.SiteGroupsAssign, currentClientId),
                        }
                    },
                    new Permission {
                        Title = "Create / Manage Personal Groups",
                        Claims =
                        {
                             new SecurityClaim(UserClaimTypes.PersonalGroupOwner, currentClientId)
                        }
                    },
                }
            });

            _permissions.Add(new Permission
            {
                Title = "Notification Groups",
                Permissions = new Permission[]
                {
                    new Permission {
                        Title = "Manage Global Notification Groups",
                        Claims =
                        {
                            new SecurityClaim(ClientClaimTypes.AppNotifyGroupsRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppNotifyGroupsCreate, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppNotifyGroupsEdit, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppNotifyGroupsDelete, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppNotifyGroupsAssign, currentClientId),
                        }
                    },
                    new Permission {
                        Title = "Send Nofifications to Global Groups",
                        Claims =
                        {
                            new SecurityClaim(ClientClaimTypes.AppNotificationsSend, currentClientId),
                        }
                    },                   
                    new Permission {
                        Title = "Manage All Site Notification Groups",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteNotifyGroupsRead, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteNotifyGroupsCreate, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteNotifyGroupsEdit, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteNotifyGroupsDelete, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteNotifyGroupsAssign, currentClientId),
                        }
                    },
                    new Permission {
                        Title = "Send Notifications to Site Groups",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteNotificationsSend, currentClientId),
                        },
                    },
                }
            });

            _permissions.Add(new Permission
            {
                Title = "Document Library",
                Permissions =
                {
                    new Permission {
                        Title = "Browse Global Document Library",
                        Claims =
                        {
                            new SecurityClaim(ClientClaimTypes.AppLibraryRead, currentClientId),
                        }
                    },
                    new Permission {
                        Title = "Manage Global Document Library",
                        Claims =
                        {
                            new SecurityClaim(ClientClaimTypes.AppLibraryRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.AppLibraryOwner, currentClientId)
                           
                        }
                    },
                    new Permission {
                        Title = "View All Sites Document Libraries",
                        Claims = {
                            new SecurityClaim(SiteClaimTypes.SiteLibraryReader, currentClientId)
                        }
                    },
                    new Permission {
                        Title = "Manage All Sites Document Libraries",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteLibraryReader, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteLibraryOwner, currentClientId)

                        }
                    },
                    new Permission {
                        Title = "Browse All User Libraries",
                        Claims =
                        {
                            new SecurityClaim(ClientClaimTypes.UserLibrariesBrowse, currentClientId),
                        }
                    },
                    new Permission {
                        Title = "Manage All User Libraries",
                        Claims =
                        {
                            new SecurityClaim(ClientClaimTypes.UserLibrariesBrowse, currentClientId),
                            new SecurityClaim(ClientClaimTypes.UserLibrariesManage, currentClientId)
                        }
                    },
                    new Permission {
                        Title = "Manage Personal Document Library",
                        Claims =
                        {
                            new SecurityClaim(UserClaimTypes.PersonalLibraryOwner, currentClientId)
                        }
                    }
                }
            });

            _permissions.Add(new Permission
            {
                Title = "Websites",
                Permissions = new Permission[]
                {
                    new Permission {
                        Title = "Manage / Create Websites",
                        Claims =
                        {
                            new SecurityClaim(ClientClaimTypes.SitesCreate, currentClientId),
                            new SecurityClaim(ClientClaimTypes.SitesRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.SitesEdit, currentClientId),
                            new SecurityClaim(ClientClaimTypes.SitesDelete, currentClientId),
                            new SecurityClaim(ClientClaimTypes.SitesPublish, currentClientId),
                        }
                    },
                    new Permission {
                        Title = "Manage / Create Site Collections",
                        Claims =
                        {
                            new SecurityClaim(ClientClaimTypes.SiteCollectionsCreate, currentClientId),
                            new SecurityClaim(ClientClaimTypes.SiteCollectionsRead, currentClientId),
                            new SecurityClaim(ClientClaimTypes.SiteCollectionsEdit, currentClientId),
                            new SecurityClaim(ClientClaimTypes.SiteCollectionsDelete, currentClientId),
                            new SecurityClaim(ClientClaimTypes.SiteCollectionsAssign, currentClientId),
                        }
                    },
                    new Permission {
                        Title = "Create / Design All Website Content",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteSettingsEdit, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteSettingsRead, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteTemplateEdit, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteTemplateRead, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SitePagesCreate, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SitePagesRead, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SitePagesEdit, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SitePagesDelete, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SitePagesDesign, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SitePagesPublish, currentClientId),

                            // HOTFIX: Per Dave - Client's cannot design master pages
                            /*
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesDelete, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesCreate, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesRead, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesEdit, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesDelete, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesDesign, currentClientId),
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesPublish, currentClientId),
                            */
                              
                        }
                    }
                }
            });        
        }

      
        public IEnumerable<Permission> PermissionGroups()
        {
            return _permissions;
        }
    }
}
