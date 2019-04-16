using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Identity.Models;

namespace Angelo.Connect.Announcement.Security
{
    public class AnnouncementSitePermissionProvider : ISecurityPermissionProvider
    {
        // using site admin context, not site context 
        private IContextAccessor<SiteAdminContext> _siteContextAccessor;

        public PoolType Level { get; } = PoolType.Site;


        public AnnouncementSitePermissionProvider(IContextAccessor<SiteAdminContext> siteContextAccessor)
        {
            _siteContextAccessor = siteContextAccessor;
        }

        public IEnumerable<Permission> PermissionGroups()
        {
            var siteContext = _siteContextAccessor.GetContext();

            if (siteContext?.Site == null)
                throw new NullReferenceException("SiteAdminContext.Site is required");

            var currentSiteId = siteContext.Site.Id;


            //build permissions groups
            return new List<Permission>(){
                    new Permission {
                        Title = "Announcement Content",
                        Permissions = new Permission[]
                        {
                            new Permission {
                                Title = "Author & Create Personal Announcements",
                                Claims = {
                                    new SecurityClaim(AnnouncementClaimTypes.PersonalAnnouncementAuthor, currentSiteId),
                                    new SecurityClaim(AnnouncementClaimTypes.PersonalAnnouncementPublish, currentSiteId),
                                }
                            },
                        }
                    }
            };
        }
    }
}
