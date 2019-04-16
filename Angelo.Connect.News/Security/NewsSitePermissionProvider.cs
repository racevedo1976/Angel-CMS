using System;
using System.Collections.Generic;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Identity.Models;

namespace Angelo.Connect.News.Security
{
    public class NewsSitePermissionProvider : ISecurityPermissionProvider
    {
        // using site admin context, not site context 
        private IContextAccessor<SiteAdminContext> _siteContextAccessor;

        public PoolType Level { get; } = PoolType.Site;


        public NewsSitePermissionProvider(IContextAccessor<SiteAdminContext> siteContextAccessor)
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
                        Title = "News Content",
                        Permissions = new[]
                        {
                            new Permission {
                                Title = "Author & Create Personal News",
                                Claims = {
                                    new SecurityClaim(NewsClaimTypes.PersonalNewsAuthor, currentSiteId),
                                    new SecurityClaim(NewsClaimTypes.PersonalNewsPublish, currentSiteId),
                                }
                            },
                        }
                    }
            };
        }
    }
}
