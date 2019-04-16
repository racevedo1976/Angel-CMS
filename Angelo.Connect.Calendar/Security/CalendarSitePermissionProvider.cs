using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Identity.Models;

namespace Angelo.Connect.Calendar.Security
{
    public class CalendarSitePermissionProvider : ISecurityPermissionProvider
    {
        // using site admin context, not site context 
        private IContextAccessor<SiteAdminContext> _siteContextAccessor;

        public PoolType Level { get; } = PoolType.Site;


        public CalendarSitePermissionProvider(IContextAccessor<SiteAdminContext> siteContextAccessor)
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
                        Title = "Calendar Content",
                        Permissions = new Permission[]
                        {
                            new Permission {
                                Title = "Create Calendars Events",
                                Claims = {
                                    new SecurityClaim(CalendarClaimTypes.CalendarEventGroupContribute, currentSiteId)
                                }
                            },
                        }
                    }
            };
        }
    }
}
