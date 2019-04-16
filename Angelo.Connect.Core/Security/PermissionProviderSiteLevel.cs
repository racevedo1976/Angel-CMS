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
    public class PermissionProviderSiteLevel : ISecurityPermissionProvider
    {
        public PoolType Level { get; } = PoolType.Site;

        private List<Permission> _permissions;

        public PermissionProviderSiteLevel(IContextAccessor<SiteAdminContext> siteContextAccessor)
        {
            _permissions = new List<Permission>();
            
            CreatePermissionGroups(siteContextAccessor.GetContext());
        }
   
        private void CreatePermissionGroups(SiteAdminContext siteContext)
        {
            if (siteContext?.Site == null)
                throw new NullReferenceException("SiteAdminContext.Site is required");

            var currentSiteId = siteContext.Site.Id;
            var currentClientId = siteContext.Site.ClientId;

            //build permissions groups
            _permissions.Add(new Permission
            {
                Title = "Users & Security",
                Permissions = new Permission[]
                {
                    new Permission {
                        Title = "Create / Manage User Profiles",
                        Claims = {
                            new SecurityClaim(SiteClaimTypes.SiteUsersRead, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteUsersCreate, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteUsersEdit, currentSiteId),
                            //new SecurityClaim(SiteClaimTypes.SiteUsersDelete, currentSiteId),
                        }
                    },

                     new Permission {
                        Title = "Manage User Security",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteUsersRead, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteRolesAssign, currentSiteId),
                        }
                    },

                    new Permission {
                        Title = "Create / Manage Site Roles",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteRolesRead, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteRolesCreate, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteRolesEdit, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteRolesDelete, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteRolesAssign, currentSiteId),
                        }
                    },

                }
            });

            _permissions.Add(new Permission
            {
                Title = "Connection Groups",
                Permissions = new Permission[]
                {
                    /*
                    new Permission {
                        Title = "Manage Site Connection Groups",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteGroupsRead, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteGroupsCreate, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteGroupsEdit, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteGroupsDelete, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteGroupsAssign, currentSiteId),
                        }
                    },
                    */
                    new Permission {
                        Title = "Create / Manage Personal Groups",
                        Claims =
                        {
                            new SecurityClaim(UserClaimTypes.PersonalGroupOwner, currentSiteId)
                        }
                    },
                }
            });

            _permissions.Add(new Permission
            {
                Title = "Notifications",
                Permissions = new Permission[]
                {
                    new Permission {
                        Title = "Manage Site Notification Groups",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteNotifyGroupsRead, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteNotifyGroupsCreate, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteNotifyGroupsEdit, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteNotifyGroupsDelete, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteNotifyGroupsAssign, currentSiteId),
                        }
                    },
                    new Permission {
                        Title = "Send Notifications to Site Groups",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteNotificationsSend, currentSiteId),
                        },
                    },
                }
            });

            _permissions.Add(new Permission
            {
                Title = "Site Library",
                Permissions =
                {
                    new Permission {
                        Title = "Browse Site Document Library",
                        Claims = {
                            new SecurityClaim(SiteClaimTypes.SiteLibraryReader, currentSiteId),
                        }
                    },
                    new Permission {
                        Title = "Manage Site Document Library",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteLibraryOwner, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteLibraryReader, currentSiteId)
                        }
                    },
                    new Permission {
                        Title = "Manage Personal Document Library",
                        Claims =
                        {
                            new SecurityClaim(UserClaimTypes.PersonalLibraryOwner, currentSiteId)
                        }
                    }
                }
            });

            _permissions.Add(new Permission
            {
                Title = "Site Settings",
                Permissions = new Permission[]
                {
                        new Permission {
                            Title = "Edit Site Settings",
                            Claims =
                            {
                                new SecurityClaim(SiteClaimTypes.SiteSettingsEdit, currentSiteId),
                                new SecurityClaim(SiteClaimTypes.SiteSettingsRead, currentSiteId),
                            }
                        },
                        new Permission {
                            Title = "Edit Site Template",
                            Claims =
                            {
                                new SecurityClaim(SiteClaimTypes.SiteTemplateEdit, currentSiteId),
                                new SecurityClaim(SiteClaimTypes.SiteTemplateRead, currentSiteId),
                            }
                        },
                        new Permission {
                            Title = "Edit Site Settings",
                            Claims =
                            {
                                new SecurityClaim(SiteClaimTypes.SiteSettingsEdit, currentSiteId),
                                new SecurityClaim(SiteClaimTypes.SiteSettingsRead, currentSiteId),
                            }
                        },
                }
            });

            _permissions.Add(new Permission
            {
                Title = "Site Pages",
                Permissions = new Permission[]
                {
                    // HOTFIX: Per Dave, Site's cannot design master pages
                    /*
                    new Permission {
                        Title = "Create / Manage Master Pages",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesCreate, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesRead, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesEdit, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesDelete, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesDesign, currentSiteId),
                        }
                    },
                    new Permission {
                        Title = "Publish Master Pages",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SiteMasterPagesPublish, currentSiteId),
                        }
                    },
                    */
                    new Permission {
                        Title = "Create / Manage Site Pages",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SitePagesCreate, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SitePagesRead, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SitePagesEdit, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SitePagesDelete, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SitePagesDesign, currentSiteId),
                        }
                    },
                    new Permission {
                        Title = "Edit Site Pages",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SitePagesRead, currentSiteId),
                            new SecurityClaim(SiteClaimTypes.SitePagesEdit, currentSiteId),
                        }
                    },
                    new Permission {
                        Title = "Publish Site Pages",
                        Claims =
                        {
                            new SecurityClaim(SiteClaimTypes.SitePagesPublish, currentSiteId),
                        }
                    },
                    new Permission {
                        Title = "Create / Manage Personal Pages",
                        Claims =
                        {
                            new SecurityClaim(UserClaimTypes.PersonalPageAuthor, currentSiteId)
                        }
                    },
                    new Permission {
                        Title = "Publish Personal Pages",
                        Claims =
                        {
                            new SecurityClaim(UserClaimTypes.PersonalPagePublish, currentSiteId),
                        }
                    },
                }
            });

        }

        public IEnumerable<Permission> PermissionGroups()
        {
            return _permissions;
        }
    }
}
